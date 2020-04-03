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
            
            //  Start database
            Console.WriteLine("Creating database.. ");

            MongoClient databaseClient = new MongoClient("mongodb://localhost:27017");
            IMongoDatabase database = databaseClient.GetDatabase("nome_db");        // Alterar esta merdita

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

        public void Register(string username, string password, string fullname)
        {
            Console.WriteLine("Registered: " + username + ' ' + password + ' ' + fullname);
        }

    }
}
