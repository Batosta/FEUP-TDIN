using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Runtime.Remoting;
using System.Text;
using System.Windows.Forms;

namespace ChatClient
{
    public partial class MainWindow : Form
    {
        // server + port
        readonly IServerObj server;
        readonly string username;
        readonly string port;

        // delegates + events
        delegate ListViewItem AddDelegate(ListViewItem item);
        delegate void RemoveDelegate(string removal, string port);

        Repeater repeater;

        RemoteObject remObj;

        // other variables
        List<UserSession> sessions;


        //  TODO Selecionar um dos active users para começar um chat com ele.
        //  TODO Criar box para enviar mensagem e funçao no ClientObj para receber mensagem e mostrá-la
        public MainWindow(IServerObj server, string username, string port)
        {

            this.server = server;
            this.username = username;
            this.port = port;

            InitializeComponent();

            sessions = server.GetActiveUsersList();
            ShowAllOnlineSessions();

            RepeaterSection();

            string url = "tcp://localhost:" + port + "/Message";
            remObj = (RemoteObject)RemotingServices.Connect(typeof(RemoteObject), url);
        }

        private void RepeaterSection()
        {
            repeater = new Repeater();
            repeater.mainDelegate += new MainDelegate(ChangeMainWindow);
            server.mainDelegate += new MainDelegate(repeater.Repeat);
        }

        private void ShowAllOnlineSessions()
        {
            foreach (UserSession session in sessions)
            {
                if (session.username != username)   // Listing active users that are not this session
                {
                    onlineSessions.Items.Add(session.username);
                    //ClientObj client = (ClientObj)RemotingServices.Connect(typeof(ClientObj), "tcp://localhost:" + session.port + "/Message");
                    //client.test("Hi. I'm " + ChatClient.username);
                }
            }

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        public void ChangeMainWindow(Action action, string username, string port)
        {
            Console.WriteLine("ChangeMainWindowChangeMainWindowChangeMainWindowChangeMainWindow");
            if (action == Action.SessionStart)
            {
                sessions.Add(new UserSession(username, port));
                // Checkar esta parte
                BeginInvoke(new AddDelegate(onlineSessions.Items.Add), new ListViewItem(username));
            }
            else if(action == Action.SessionEnd)
            {
                // Checkar esta parte
                BeginInvoke(new RemoveDelegate(RemoveSessionFromSessions), new object[] { username, port });
            }
        }

        private void RemoveSessionFromSessions(string username, string port)
        {
            UserSession endedUserSession = new UserSession(username, port);
            sessions.Remove(endedUserSession);
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            //Console.WriteLine("Load MainWindow");
            //List<UserSession> sessions = serverObj.GetActiveUsersList();
            //Console.WriteLine(sessions.Count);
            //foreach (UserSession session in sessions)
            //{
            //    Console.WriteLine(session.username);
            //    Console.WriteLine(session.port);

            //    // Listing active users that are not this session
            //    if (session.username != ChatClient.username)
            //    {
            //        listBox1.Items.Add("User " + session.username);
            //        ClientObj client = (ClientObj)RemotingServices.Connect(typeof(ClientObj), "tcp://localhost:" + session.port + "/Message");
            //        client.test("Hi. I'm " + ChatClient.username);
            //    }
            //}
        }
    }

    public class RemoteObject : MarshalByRefObject, IClientObj
    {
        private MainWindow mainWindow;

        public override object InitializeLifetimeService()
        {
            Console.WriteLine("passa aqui 3");
            return null;
        }
    }
}
