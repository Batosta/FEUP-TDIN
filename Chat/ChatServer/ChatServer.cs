using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;  // To use the MongoDB.Driver we need to add a directive
using System.Runtime.Remoting;

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
        MongoClient databaseClient;
        IMongoDatabase database;

        public void Register(string username, string realName, string password)
        {
            databaseClient = new MongoClient("mongodb://localhost:27017");
            database = databaseClient.GetDatabase("serverDB");

            var collection = database.GetCollection<UserModel>("User");
            collection.InsertOne(new UserModel { Username = username, RealName = realName, Password = password });
            Console.WriteLine("User Registered");
        }

    }
}
