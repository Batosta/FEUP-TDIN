using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using System.Collections;

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

            IServerObj obj = (IServerObj) Activator.GetObject(typeof(IServerObj),
                  "tcp://localhost:9000/ChatServer/Rem");

            obj.Register("migumigu", "passworddemerda", "Miguel Dias de Carvalho");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Login());

        }
    }
}
