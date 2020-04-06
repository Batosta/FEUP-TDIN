using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Runtime.Remoting;
using System.Text;
using System.Windows.Forms;

namespace Chat
{
    public partial class MainWindow : Form
    {
        readonly IServerObj serverObj;

        //  TODO Selecionar um dos active users para começar um chat com ele.
        //  TODO Criar box para enviar mensagem e funçao no ClientObj para receber mensagem e mostrá-la
        public MainWindow(IServerObj serverObj)
        {
            this.serverObj = serverObj;
            InitializeComponent();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            Console.WriteLine("Load MainWindow");
            List<UserSession> sessions = serverObj.GetActiveUsersList();
            Console.WriteLine(sessions.Count);
            foreach (UserSession session in sessions)
            {
                Console.WriteLine(session.username);
                Console.WriteLine(session.host);
                Console.WriteLine(session.port);

                // Listing active users that are not this session
                if(session.username != Chat.username)
                {
                    listBox1.Items.Add("User " + session.username);
                    ClientObj client = (ClientObj)RemotingServices.Connect(typeof(ClientObj), "tcp://localhost:" + session.port + "/Message");
                    client.test("Hi. I'm " + Chat.username);
                }
            }
        }
    }
}
