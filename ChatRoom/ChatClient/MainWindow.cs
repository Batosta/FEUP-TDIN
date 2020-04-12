using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChatClient
{
    public partial class MainWindow : Form
    {
        // server + port
        IServerObj server;
        readonly string username;
        readonly string port;

        // delegates + events
        delegate ListViewItem LVAddDelegate(ListViewItem lvItem);
        delegate void LVRemDelegate(string lvItem);
        AlterEventRepeater evRepeater;

        // other variables
        List<UserSession> activeSessions;
        List<ConversationWindow> activeConversationWindows;

        public MainWindow(IServerObj server, string username, string port)
        {
            InitializeComponent();
            this.Text = username;       // change the window name to the username for more easy recognition when multiple accounts open on the same computer

            this.server = server;
            this.username = username;
            this.port = port;

            this.activeConversationWindows = new List<ConversationWindow>();

            PlaceActiveSessions();
            AlterEventRepeaterSection();
        }

        private void PlaceActiveSessions()
        {
            this.activeSessions = server.GetActiveSessions();
            activeSessions = server.GetActiveSessions();
            foreach (UserSession activeSession in activeSessions)
            {
                if (activeSession.username != username)
                {
                    activeSessionsList.Items.Add(activeSession.username);
                }
            }
        }

        private void AlterEventRepeaterSection()
        {
            evRepeater = new AlterEventRepeater();
            evRepeater.alterEvent += new AlterDelegate(DoAlterations);
            server.alterEvent += new AlterDelegate(evRepeater.Repeater);
        }




        public void DoAlterations(Operation op, string username, string port)
        {
            LVAddDelegate lvAdd;
            LVRemDelegate lvRem;

            switch (op)
            {
                case Operation.SessionStart:

                    UserSession newUserSession = new UserSession(username, port);
                    activeSessions.Add(newUserSession);
                    lvAdd = new LVAddDelegate(activeSessionsList.Items.Add);
                    ListViewItem lvItem = new ListViewItem(new string[] { username });
                    // Checkar esta parte
                    BeginInvoke(lvAdd, new object[] { lvItem });
                    //lvAdd.Invoke(lvItem);       // como o stor tem
                    break;

                case Operation.SessionEnd:
                    Console.WriteLine("DoAlterations for Operation.SessionEnd");
                    break;

                default:
                    break;
            }
        }

        private void start_conversation_Click(object sender, EventArgs e)
        {
            string selectedUsername = activeSessionsList.SelectedItems[0].Text;
            ConversationWindow newConversationWindow = new ConversationWindow(server, selectedUsername);
            activeConversationWindows.Add(newConversationWindow);
            newConversationWindow.Show();
        }
    }




    public class RemMessage : MarshalByRefObject, IClientObj
    {
        private MainWindow win;

        public override object InitializeLifetimeService()
        {
            return null;
        }

        public void PutMyForm(MainWindow form)
        {
            win = form;
        }
    }
}
