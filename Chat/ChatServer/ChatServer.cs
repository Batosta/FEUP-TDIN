using System;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Threading;
using MongoDB.Driver;  // To use the MongoDB.Driver we need to add a directive

namespace ChatServer
{
    class ChatServer
    {
        static void Main(string[] args)
        {
            //  Register server
            Console.WriteLine("Registering server.. ");
            RemotingConfiguration.Configure("ChatServer.exe.config", false);
            RemotingConfiguration.RegisterWellKnownServiceType(typeof(ServerObj), "Rem", WellKnownObjectMode.Singleton);
            Console.WriteLine("\nPress Return to terminate.");
            Console.ReadLine();
        }
    }

    public class ServerObj : MarshalByRefObject, IServerObj
    {
        static MongoClient databaseClient = new MongoClient("mongodb://localhost:27017");
        readonly IMongoDatabase database = databaseClient.GetDatabase("serverDB");

        public event MainDelegate mainDelegate;


        List<UserSession> sessions = new List<UserSession>();

        public int Register(string username, string realName, string password)
        {
            Console.WriteLine("Received Register. username: " + username + " realname: " + realName);
            var collection = database.GetCollection<UserModel>("User");

            //  Check if username already exists
            var result = collection.Find(x => x.Username == username).FirstOrDefault();
            if (result != null) return -1;

            UserModel newUser = new UserModel
            {
                Username = username,
                RealName = realName,
                Password = password
            };
            collection.InsertOne(newUser);
            Console.WriteLine("User Registered");
            return 1;
        }

        /*
         * Return Values:
         * 1 - Correct username and password
         * 2 - Correct username and password, but user already logged in
         * 3 - Correct Username, but wrong password
         * 4 - Wrong username and password
         */
        public int Login(string username, string password, string port)
        {
            var collection = database.GetCollection<UserModel>("User");
            var filter = Builders<UserModel>.Filter.Eq("Username", username);
            var user = collection.Find(filter).FirstOrDefault();

            if (user != null)   // Checks if user with that username exists
            {
                if (PasswordHandler.Validate(password, user.Password))     // Checks if correct password for valid username
                {
                    if (!sessions.Contains(new UserSession(username, port)))      // Checks if user is not already logged in
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
            string url = "tcp://localhost:" + port + "/Message";
            UserSession newUserSession = new UserSession(username, port);

            IClientObj remoteClient = (IClientObj)RemotingServices.Connect(typeof(IClientObj), url);
            sessions.Add(newUserSession);
            AlertAllClients(Action.SessionStart, username, port);
        }

        private void AlertAllClients(Action action, string username, string port)
        {
            // Checkar esta parte
            if(mainDelegate != null)
            {
                // An array of delegates representing the invocation list of the current delegate.
                Delegate[] delegates = mainDelegate.GetInvocationList();

                foreach (MainDelegate singleDelegate in delegates)
                {
                    new Thread(() =>
                    {
                        try
                        {
                            Console.WriteLine("trytrytrytrytrytrytrytrytrytry");
                            singleDelegate(action, username, port);
                        }
                        catch(Exception exception)
                        {
                            Console.WriteLine("exceptionexceptionexceptionexception");
                            mainDelegate -= singleDelegate;
                            Console.WriteLine("Exception: " + exception);
                        }
                    }).Start();

                }

            }
        } 

        public string HashPassword(string password)
        {
            return PasswordHandler.CreatePasswordHash(password);
        }

        public List<UserSession> GetActiveUsersList()
        {
            return sessions;
        }
    }
}
