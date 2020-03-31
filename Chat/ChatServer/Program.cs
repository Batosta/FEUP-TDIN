using System;
using MongoDB.Driver;  // To use the MongoDB.Driver we need to add a directive
using MongoDB.Bson;

namespace ChatServer
{
    class Program
    {
        static void Main(string[] args)
        {
            MongoClient databaseClient = new MongoClient("mongodb://localhost:27017");
            //IMongoDatabase database = databaseClient.GetDatabase("nome_db");        // Alterar esta merdita

            var dbList = databaseClient.ListDatabases().ToList();

            Console.WriteLine("The list of databases on this server is: ");
            foreach (var db in dbList)
            {
                Console.WriteLine(db);
            }
        }
    }
}
