using System;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Threading;
using MongoDB.Driver;

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
                if (!(activeSessions.Contains(new UserSession(username, port))))
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
        IClientObj iClientObj = (IClientObj)RemotingServices.Connect(typeof(IClientObj), address);      // Obtain a reference to the client remote object

        UserSession newUserSession = new UserSession(username, port);
        activeSessions.Add(newUserSession);

        NotifyClients(Operation.SessionStart, username, port);
    }

    public List<UserSession> GetActiveSessions()
    {
        return activeSessions;
    }








    public string HashPassword(string password)
    {
        return PasswordHandler.CreatePasswordHash(password);
    }

    void NotifyClients(Operation op, string username, string port)
    {
        if(alterEvent != null)
        {
            Delegate[] invkList = alterEvent.GetInvocationList();

            foreach(AlterDelegate handler in invkList)
            {
                new Thread(() =>
                {
                    try
                    {
                        handler(op, username, port);
                    }
                    catch(Exception exception)
                    {
                        alterEvent -= handler;
                        Console.WriteLine("[Exception]: " + exception);
                    }
                }).Start();
            }
        }
    }
}
