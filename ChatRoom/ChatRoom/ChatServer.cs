using MongoDB.Driver;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;
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
    ConcurrentDictionary<string, Dictionary<string, bool>> groupProposals = new ConcurrentDictionary<string, Dictionary<string, bool>>();


    /*
     * Login + Register methods
     */
    /*
    * Return values:
    * 1 - Correct username and password
    * 2 - Correct username and password, but user already logged it
    * 3 - Correct username, but wrong password
    * 4 - Non-existent username
    */
    public int Login(string username, string password, string port)
    {
        var collection = database.GetCollection<UserModel>("User");
        var filter = Builders<UserModel>.Filter.Eq("Username", username);
        var user = collection.Find(filter).FirstOrDefault();

        if (user != null)
        {
            if (PasswordHandler.Validate(password, user.Password))
            {
                if (!activeSessions.Contains(new UserSession(username, port)))
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

    public void PerformLogin(string username, string port)
    {
        string address = "tcp://localhost:" + port + "/Message";

        // Checkar esta parte (acho nao ser necessaria)
        //IClientObj iClientObj = (IClientObj)RemotingServices.Connect(typeof(IClientObj), address);      // Obtain a reference to the client remote object

        UserSession newUserSession = new UserSession(username, port);
        activeSessions.Add(newUserSession);

        NotifyClients(Operation.SessionStart, username, port);
    }

    /*
    * Return values:
    * -1 - Already existent username
    * 1 - Successful register
    */
    public int Register(string username, string realName, string password)
    {
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
        return 1;
    }

    public void SendGroupProposal(string proposalSenderUsername, List<string> proposalReceiverUsernames)
    {
        Dictionary<string, bool> usersToAccept = new Dictionary<string, bool>();
        foreach (string receiverUsername in proposalReceiverUsernames)
        {
            usersToAccept[receiverUsername] = false;
            string receiverAddress = getUserAddress(receiverUsername);
            IClientObj clientObjReceiver = (IClientObj)RemotingServices.Connect(typeof(IClientObj), (string)receiverAddress);

            new Thread(() =>
            {
                clientObjReceiver.ReceiveGroupProposal(proposalSenderUsername, proposalReceiverUsernames);
            }).Start();
        }
        groupProposals[proposalSenderUsername] = usersToAccept;
    }
    public void YesToGroupProposal(string proposalSenderUsername, string proposalReceiverUsername)
    {
        List<string> addresses = new List<string>();
        List<string> usernames = new List<string>();
        Console.WriteLine("Yes to group proposal from: " + proposalSenderUsername);
        groupProposals[proposalSenderUsername][proposalReceiverUsername] = true;
        usernames.Add(proposalSenderUsername);
        addresses.Add(getUserAddress(proposalSenderUsername));
        foreach (KeyValuePair<string, bool> entry in groupProposals[proposalSenderUsername])
        {
            Console.WriteLine(entry);
            usernames.Add(entry.Key);
            addresses.Add(getUserAddress(entry.Key));
            if (entry.Value == false)
            {
                return;
            }
        }

        Console.WriteLine("All accepted. Notifying everyone...");

        IClientObj clientObjSender = (IClientObj)RemotingServices.Connect(typeof(IClientObj), (string)getUserAddress(proposalSenderUsername));

        string chatID = "";
        for (int i = 0; i < usernames.Count; i++)
        {
            chatID += usernames[i];
        }
        chatID = String.Concat(chatID.OrderBy(c => c));
        Console.WriteLine("chat id group: " + chatID);

        //  Create chat in database
        var collection = database.GetCollection<ChatModel>("Chat");
        ChatModel chatmodel = collection.Find(chat => chat.ChatID == chatID).FirstOrDefault();
        if (chatmodel == null)
        {
            List<string> users = new List<string>();
            List<MessageModel> messages = new List<MessageModel>();
            foreach(string receiverUsername in usernames)
            {
                users.Add(receiverUsername);
            }
            collection.InsertOne(new ChatModel
            {
                ChatID = chatID,
                Users = users,
                Messages = messages
            });
            Console.WriteLine("Chat ID created and added to database: " + chatID);

            foreach (KeyValuePair<string, bool> entry in groupProposals[proposalSenderUsername])
            {
                IClientObj clientObjReceiver = (IClientObj)RemotingServices.Connect(typeof(IClientObj), (string)getUserAddress(entry.Key));
                clientObjReceiver.StartGroupChat(usernames, addresses, chatID, null);
            }

            // Events?

            clientObjSender.StartGroupChat(usernames, addresses, chatID, null);
        }
        else
        {
            foreach (KeyValuePair<string, bool> entry in groupProposals[proposalSenderUsername])
            {
                IClientObj clientObjReceiver = (IClientObj)RemotingServices.Connect(typeof(IClientObj), (string)getUserAddress(entry.Key));
                clientObjReceiver.StartGroupChat(usernames, addresses, chatID, chatmodel);
            }

            // Events?

            clientObjSender.StartGroupChat(usernames, addresses, chatID, chatmodel);
        }
    }

    public void NoToGroupProposal(string proposalSenderUsername, string proposalReceiverUsername)
    {
        throw new NotImplementedException();
    }

    /*
     * Conversation Initiation methods
     */
    public void SendProposal(string proposalSenderUsername, string proposalReceiverUsername)
    {

        Console.WriteLine("proposalSenderUsername: " + proposalSenderUsername);
        Console.WriteLine("proposalReceiverUsername: " + proposalReceiverUsername);
        string proposalReceiverAddress = getUserAddress(proposalReceiverUsername);
        Console.WriteLine("proposalReceiverAddress: " + proposalReceiverAddress);


        // CRASHOU AQUI
        IClientObj clientObjReceiver = (IClientObj)RemotingServices.Connect(typeof(IClientObj), (string)proposalReceiverAddress);
        clientObjReceiver.ReceiveProposal(proposalSenderUsername);
    }

    public void YesToProposal(string proposalSenderUsername, string proposalReceiverUsername)
    {
        string proposalSenderAddress = getUserAddress(proposalSenderUsername);
        string proposalReceiverAddress = getUserAddress(proposalReceiverUsername);

        IClientObj clientObjSender = (IClientObj)RemotingServices.Connect(typeof(IClientObj), (string)proposalSenderAddress);
        IClientObj clientObjReceiver = (IClientObj)RemotingServices.Connect(typeof(IClientObj), (string)proposalReceiverAddress);

        string chatID = proposalSenderUsername + proposalReceiverUsername;
        chatID = String.Concat(chatID.OrderBy(c => c));

        //  Create chat in database
        var collection = database.GetCollection<ChatModel>("Chat");
        ChatModel chatmodel = collection.Find(chat => chat.ChatID == chatID).FirstOrDefault();
        if (chatmodel == null)
        {
            List<string> users = new List<string>();
            List<MessageModel> messages = new List<MessageModel>();
            users.Add(proposalSenderUsername);
            users.Add(proposalReceiverUsername);
            collection.InsertOne(new ChatModel
            {
                ChatID = chatID,
                Users = users,
                Messages = messages
            });
            Console.WriteLine("Chat ID created and added to database: " + chatID);

            clientObjSender.ReceiveYesToProposal(proposalReceiverUsername, proposalReceiverAddress, chatID, null);
            clientObjReceiver.StartAcceptedProposal(proposalSenderUsername, proposalSenderAddress, chatID, null);
        } else
        {
            clientObjSender.ReceiveYesToProposal(proposalReceiverUsername, proposalReceiverAddress, chatID, chatmodel);
            clientObjReceiver.StartAcceptedProposal(proposalSenderUsername, proposalSenderAddress, chatID, chatmodel);
        }
    }

    public void NoToProposal(string proposalSenderUsername, string proposalReceiverUsername)
    {
        string proposalSenderAddress = getUserAddress(proposalSenderUsername);

        IClientObj clientObjSender = (IClientObj)RemotingServices.Connect(typeof(IClientObj), (string)proposalSenderAddress);
        clientObjSender.ReceiveNoToProposal(proposalReceiverUsername);
    }


    public void storeMessage(MessageModel message)
    {
        var collection = database.GetCollection<ChatModel>("Chat");
        var update = Builders<ChatModel>.Update.Push("Messages", message);
        collection.FindOneAndUpdate(chat => chat.ChatID == message.ChatID, update);
    }


    public List<UserSession> GetActiveSessions()
    {
        return activeSessions;
    }

    private string getUserAddress(string username)
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
