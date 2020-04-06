/*
 * The code for the hashing of the passwords was taken from https://dotnetfiddle.net/yVeOVA
 */

using System;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Security.Cryptography;
using System.Text;
using MongoDB.Bson;
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
        readonly MongoClient databaseClient;
        readonly IMongoDatabase database;
        string[] activeUsers;    //  Probably more than strings
        List<UserSession> sessions = new List<UserSession>();

        public ServerObj()
        {
            databaseClient = new MongoClient("mongodb://localhost:27017");
            database = databaseClient.GetDatabase("serverDB");
            Console.WriteLine("Connected to the database");
        }

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
         * 2 - Correct Username, but wrong password
         * 3 - Wrong username and password
         */
        public int Login(string username, string password, string host, string port)
        {

            Console.WriteLine("Login from: " + host + '/' + port + ", " + username + ", " + password);
            sessions.Add(new UserSession(username, host, port));

            foreach (UserSession session in sessions)
            {
                Console.WriteLine("session: " + session.username + " host: " + session.host);
            }

            var collection = database.GetCollection<UserModel>("User");
            var filter = Builders<UserModel>.Filter.Eq("Username", username);
            var user = collection.Find(filter).FirstOrDefault();

            if (user != null)
            {
                if (PasswordHandler.Validate(password, user.Password))
                    return 1;
                else
                    return 2;
            }
            else
                return 3;
        }

        public string HashPassword(string password)
        {
            return PasswordHandler.CreatePasswordHash(password);
        }

        public List<UserSession> GetActiveUsersList()
        {
            Console.WriteLine("GetActiveUsers called.");
            return sessions;
        }
    }

    public class PasswordHandler
    {
        public static string CreatePasswordHash(string pwd)
        {
            return CreatePasswordHash(pwd, CreateSalt());
        }

        public static string CreatePasswordHash(string pwd, string salt)
        {
            string saltAndPwd = String.Concat(pwd, salt);
            string hashedPwd = GetHashString(saltAndPwd);
            var saltPosition = 5;
            hashedPwd = hashedPwd.Insert(saltPosition, salt);
            return hashedPwd;
        }

        public static bool Validate(string password, string passwordHash)
        {
            var saltPosition = 5;
            var saltSize = 10;
            var salt = passwordHash.Substring(saltPosition, saltSize);
            var hashedPassword = CreatePasswordHash(password, salt);
            return hashedPassword == passwordHash;
        }

        private static string CreateSalt()
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] buff = new byte[20];
            rng.GetBytes(buff);
            var saltSize = 10;
            string salt = Convert.ToBase64String(buff);
            if (salt.Length > saltSize)
            {
                salt = salt.Substring(0, saltSize);
                return salt.ToUpper();
            }

            var saltChar = '^';
            salt = salt.PadRight(saltSize, saltChar);
            return salt.ToUpper();
        }

        private static string GetHashString(string password)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in GetHash(password))
                sb.Append(b.ToString("X2"));
            return sb.ToString();
        }

        private static byte[] GetHash(string password)
        {
            SHA384 sha = new SHA384CryptoServiceProvider();
            return sha.ComputeHash(Encoding.UTF8.GetBytes(password));
        }
    }
}
