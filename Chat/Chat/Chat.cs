using System;
using System.Windows.Forms;

namespace Chat
{
    static class Chat
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Connection with server
            Console.WriteLine("Connecting to server...");
            IServerObj iServerObj = (IServerObj) Activator.GetObject(typeof(IServerObj),
                  "tcp://localhost:9000/ChatServer/Rem");
            Console.WriteLine("Connected to server...");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Login(iServerObj));
        }
    }
}
