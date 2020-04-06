using System;
using System.Collections;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Windows.Forms;

namespace Chat
{
    static class Chat
    {
        public static string host;
        public static string port;
        public static string username;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Connection with server
            Console.WriteLine("Connecting to server...");
            IServerObj iServerObj = (IServerObj)Activator.GetObject(typeof(IServerObj),
                  "tcp://localhost:9000/ChatServer/Rem");
            Console.WriteLine("Connected to server");

            TcpChannel chan = new TcpChannel(0);  // instantiate the channel with port dynamically assigned
            ChannelDataStore data = (ChannelDataStore)chan.ChannelData;
            port = new Uri(data.ChannelUris[0]).Port.ToString();                            // get the port
            host = new Uri(data.ChannelUris[0]).Host;

            Console.WriteLine("Host: " + host);
            Console.WriteLine("Port: " + port);
             
            RemotingConfiguration.RegisterWellKnownServiceType( typeof(ClientObj), "Message", WellKnownObjectMode.Singleton);  // register my remote object for service

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Login(iServerObj));
        }
    }

    public class ClientObj : MarshalByRefObject, IClientObj
    {
        public void test(string test)
        {
            Console.WriteLine("Received message: " + test);
        }
    }
}
