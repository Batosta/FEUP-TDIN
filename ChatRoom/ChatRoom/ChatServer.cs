﻿using MongoDB.Driver;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Threading;

class ChatServer
{
    static void Main(string[] args)
    {
        // Register Server
        Console.WriteLine("Registering server.. ");
        RemotingConfiguration.Configure("ChatRoom.exe.config", false);
        Console.WriteLine("[Server]: Press Return to terminate.");
        Console.ReadLine();
    }
}



public class ServerObj : MarshalByRefObject, IServerObj
{
    static MongoClient databaseClient = new MongoClient("mongodb://localhost:27017");
    readonly IMongoDatabase database = databaseClient.GetDatabase("serverDB");

    public event AlterDelegate alterEvent;

    List<UserSession> activeSessions = new List<UserSession>();
    ConcurrentDictionary<string, Dictionary<string, bool>> conversationProposals = new ConcurrentDictionary<string, Dictionary<string, bool>>();


    /*
     * Login + Logout + Register methods
     */
    public int Login(string username, string password, string port)
    {
        /*
        * Return values:
        * 1 - Correct username and password
        * 2 - Correct username and password, but user already logged it
        * 3 - Correct username, but wrong password
        * 4 - Non-existent username
        */
        var collection = database.GetCollection<UserModel>("User");
        var filter = Builders<UserModel>.Filter.Eq("Username", username);
        var user = collection.Find(filter).FirstOrDefault();

        if (user != null)
        {
            if (PasswordHandler.Validate(password, user.Password))
            {
                if (!UserIsActive(username))
                    return 1;
                else
                    return 2;
            }
            else
                return 3;
        }
        else
            return 4;
    }

    private bool UserIsActive(string username)
    {
        foreach (UserSession session in activeSessions)
        {
            if (session.username.Equals(username))
                return true;
        }
        return false;
    }

    public void PerformLogin(string username, string port)
    {
        string address = "tcp://localhost:" + port + "/Message";

        UserSession newUserSession = new UserSession(username, port);
        activeSessions.Add(newUserSession);

        NotifyClients(Operation.SessionStart, username, port);
        Console.WriteLine("User logged in: " + username + " in port: " + port);
    }

    public int Register(string username, string realName, string password)
    {
        /*
        * Return values:
        * -1 - Already existent username
        * 1 - Successful register
        */

        var collection = database.GetCollection<UserModel>("User");

        // check if username already exists
        var result = collection.Find(x => x.Username == username).FirstOrDefault();
        if (result != null)
            return -1;

        UserModel newUserModel = new UserModel
        {
            Username = username,
            RealName = realName,
            Password = HashPassword(password)
        };
        collection.InsertOne(newUserModel);
        Console.WriteLine("User registered: " + username);
        return 1;
    }

    public void Logout(string username)
    {
        for(int i = 0; i < activeSessions.Count; i++)
        {
            if (activeSessions[i].username.Equals(username))
            {
                activeSessions.RemoveAt(i);
                break;
            }
        }
        NotifyClients(Operation.SessionEnd, username, null);
        Console.WriteLine("User logged out: " + username);
    }


    /*
     * Conversation Initiation methods
     */
    public void SendConversationProposal(string proposalSenderUsername, List<string> proposalReceiverUsernames)
    {
        Dictionary<string, bool> usersToAcceptProposal = new Dictionary<string, bool>();
        foreach (string proposalReceiverUsername in proposalReceiverUsernames)
        {
            usersToAcceptProposal[proposalReceiverUsername] = false;
            string proposalReceiverAddress = GetUserAddress(proposalReceiverUsername);
            IClientObj clientObjReceiver = (IClientObj)RemotingServices.Connect(typeof(IClientObj), (string)proposalReceiverAddress);

            new Thread(() =>
            {
                clientObjReceiver.ReceiveProposal(proposalSenderUsername, proposalReceiverUsernames);
            }).Start();
        }
        conversationProposals[proposalSenderUsername] = usersToAcceptProposal;
    }

    public void YesToProposal(string proposalSenderUsername, string proposalReceiverUsername)
    {
        Console.WriteLine("Yes received from " + proposalReceiverUsername);
        List<string> usernames = new List<string>();
        List<string> addresses = new List<string>();
        string proposalSenderAddress = GetUserAddress(proposalSenderUsername);

        usernames.Add(proposalSenderUsername);
        addresses.Add(proposalSenderAddress);
        conversationProposals[proposalSenderUsername][proposalReceiverUsername] = true;

        foreach(KeyValuePair<string, bool> entry in conversationProposals[proposalSenderUsername])
        {
            if (entry.Value == false)
                return;
            usernames.Add(entry.Key);
            addresses.Add(GetUserAddress(entry.Key));
        }
        Console.WriteLine("All users accepted " + proposalSenderUsername + "'s proposal.");
        // If reaches this code, all receivers have already accepted the conversation
        IClientObj clientObjSender = (IClientObj)RemotingServices.Connect(typeof(IClientObj), (string)proposalSenderAddress);

        List<MessageModel> previousMessages = new List<MessageModel>();
        string chatName = GetChatNameWithUsernames(usernames);
        ChatModel existentChat = SearchForChat(chatName);
        if (existentChat == null)
        {
            CreateChatInDB(chatName, usernames);
            previousMessages = null;
        }
        else
            previousMessages = existentChat.Messages;
            
        foreach(KeyValuePair<string, bool> entry in conversationProposals[proposalSenderUsername])
        {
            string proposalReceiverAddress = GetUserAddress(entry.Key);
            new Thread(() => {
                IClientObj clientObjReceiver = (IClientObj)RemotingServices.Connect(typeof(IClientObj), (string)proposalReceiverAddress);
                Console.WriteLine("Telling " + entry.Key + " to start chat.");
                clientObjReceiver.StartChat(chatName, usernames, addresses, previousMessages);
            }).Start();
        }
        new Thread(() => { 
                Console.WriteLine("Telling " + proposalSenderUsername + " to start chat.");
                clientObjSender.StartChat(chatName, usernames, addresses, previousMessages); 
        }).Start();
    }
    public void NoToProposal(string proposalSenderUsername, string proposalReceiverUsername)
    {
    }

    public void StoreMessage(string chatName, string username, string messageText, string messageTime, bool isPrivate, List<string> otherUsernames)
    {
        var collection = database.GetCollection<ChatModel>("Chat");
        MessageModel newMessageModel = new MessageModel
        {
            ChatName = chatName,
            Sender = username,
            Text = messageText,
            Time = messageTime,
            isPrivate = isPrivate,
            Receivers = otherUsernames
        };
        var update = Builders<ChatModel>.Update.Push("Messages", newMessageModel);
        collection.FindOneAndUpdate(chat => chat.Name == newMessageModel.ChatName, update);
    }
    

    public List<UserSession> GetActiveSessions()
    {
        return activeSessions;
    }


    // Creates a new instance of a ChatModel in the database
    private void CreateChatInDB(string chatName, List<string> usernames)
    {
        var collection = database.GetCollection<ChatModel>("Chat");

        List<MessageModel> messages = new List<MessageModel>();
        ChatModel newChatModel = new ChatModel
        {
            Name = chatName,
            Users = usernames,
            Messages = messages
        };
        collection.InsertOne(newChatModel);
    }

    // Builds the name of a chat using its users' usernames
    private string GetChatNameWithUsernames(List<string> usernames)
    {
        string chatName = "";
        for (int i = 0; i < usernames.Count; i++)
        {
            chatName += usernames[i];
        }
        chatName = String.Concat(chatName.OrderBy(c => c));

        return chatName;
    }

    // Searches for a chat in the db by its name
    private ChatModel SearchForChat(string chatName)
    {
        var collection = database.GetCollection<ChatModel>("Chat");
        return collection.Find(x => x.Name == chatName).FirstOrDefault();
    }

    // Gets an user address using its username
    private string GetUserAddress(string username)
    {
        foreach (UserSession activeSession in activeSessions)
        {
            if (activeSession.username == username)
            {
                string url = "tcp://localhost:" + activeSession.port + "/Message";
                return url;
            }
        }
        return null;
    }

    // Method to hash a password
    private string HashPassword(string password)
    {
        return PasswordHandler.CreatePasswordHash(password);
    }

    void NotifyClients(Operation op, string username, string port)
    {
        if (alterEvent != null)
        {
            Delegate[] invkList = alterEvent.GetInvocationList();

            foreach (AlterDelegate handler in invkList)
            {
                new Thread(() =>
                {
                    try
                    {
                        handler(op, username, port);
                    }
                    catch (Exception exception)
                    {
                        alterEvent -= handler;
                        Console.WriteLine("[Exception]: " + exception);
                    }
                }).Start();
            }
        }
    }
}
