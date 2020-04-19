using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Threading;
using System.Windows.Forms;

namespace ChatClient
{
    public partial class MainWindow : Form
    {
        // server + port
        IServerObj server;
        public string username;
        readonly string port;

        // delegates + events
        delegate ListViewItem LVAddDelegate(ListViewItem lvItem);
        delegate void LVRemDelegate(string lvItem);
        AlterEventRepeater evRepeater;

        // other variables
        List<UserSession> activeSessions;
        public List<IChatWindow> activeConversationWindows;
        RemMessage remMessage;

        public MainWindow(IServerObj server, string username, string port)
        {
            InitializeComponent();
            this.Text = username;       // change the window name to the username for more easy recognition when multiple accounts open on the same computer

            this.server = server;
            this.username = username;
            this.port = port;

            activeConversationWindows = new List<IChatWindow>();

            //  Sessions list settings
            activeSessionsList.View = View.Details;
            ColumnHeader header = new ColumnHeader();
            activeSessionsList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            header.Text = "";
            header.Name = "users";
            header.Width = activeSessionsList.Width;
            activeSessionsList.Columns.Add(header);
            // Then
            activeSessionsList.Scrollable = true;

            PlaceActiveSessions();
            AlterEventRepeaterSection();
            SetupCommunication();
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

        private void SetupCommunication()
        {
            string url = "tcp://localhost:" + port + "/Message";
            remMessage = (RemMessage)RemotingServices.Connect(typeof(RemMessage), url);
            remMessage.PutMyForm(this);
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
                    BeginInvoke(lvAdd, new object[] { lvItem });
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
            if(activeSessionsList.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please choose at least one user to chat with.");
                return;
            }
            else
            {
                List<string> selectUsernames = new List<string>();
                foreach(ListViewItem selectedUsername in activeSessionsList.SelectedItems)
                {
                    selectUsernames.Add(selectedUsername.Text);
                }
                SendProposal(selectUsernames);
            }
        }

        private void SendProposal(List<string> proposalReceiverUsernames)
        {
            ConversationProposal conversationProposal = new ConversationProposal(server, username, proposalReceiverUsernames);
            new Thread(() =>
            {
                conversationProposal.SendConversationProposal();
            }).Start();
        }

        public void ReceiveProposal(string proposalSenderUsername, List<string> proposalReceiverUsernames)
        {
            string message = BuildConversationProposalMessage(proposalSenderUsername, proposalReceiverUsernames);
            string caption = "Conversation Proposal";
            DialogResult proposalResult = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (proposalResult == DialogResult.Yes)       // If the proposal receiver answers Yes
                server.YesToProposal(proposalSenderUsername, username);
            else                                         // If the proposal receiver answers No
                server.NoToProposal(proposalSenderUsername, username);
        }

        public delegate void OpeningConversation(Form chatwindow);

        private void OpenConversation(Form chatwindow)
        {
            if (this.InvokeRequired)
                this.Invoke(new OpeningConversation(OpenConversation), chatwindow);

            else
            {
                chatwindow.Show();
            }
        }

        public void StartChat(string chatName, List<string> usernames, List<string> addresses, List<MessageModel> previousMessages)
        {
            for(int i = 0; i < usernames.Count; i++)
            {
                if (usernames[i].Equals(username))
                {
                    usernames.RemoveAt(i);
                    addresses.RemoveAt(i);
                    break;
                }
            }

            ConversationWindow conversationWindow = new ConversationWindow(this, server, chatName, username, usernames, addresses, previousMessages);
            activeConversationWindows.Add(conversationWindow);
            OpenConversation(conversationWindow);
        }


        // Builds the proposal message for both simple and group chats
        private string BuildConversationProposalMessage(string proposalSenderUsername, List<string> proposalReceiverUsernames)
        {
            string message = "";
            if(proposalReceiverUsernames.Count == 1)
            {
                message = proposalSenderUsername + " is proposing a conversation. Do you also want to chat with him/her?";
            }
            else
            {
                string others = "";
                foreach (string proposalReceiverUsername in proposalReceiverUsernames)
                {
                    if (proposalReceiverUsername != username)
                    {
                        others += (proposalReceiverUsername + ", ");
                    }
                }
                others = others.Remove(others.Length - 2, 2);
                message = proposalSenderUsername + " is proposing a group conversation with " + others + ". Do you also want to chat with him/her?";
            }

            return message;
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

        public void ReceiveProposal(string proposalSenderUsername, List<string> proposalReceiverUsernames)
        {
            win.ReceiveProposal(proposalSenderUsername, proposalReceiverUsernames);
        }

        public void StartChat(string chatName, List<string> usernames, List<string> addresses, List<MessageModel> previousMessages)
        {
            win.StartChat(chatName, usernames, addresses, previousMessages);
        }

        public void ReceiveMessage(string chatName, string username, string messageText, string messageTime)
        {
            IChatWindow window = win.activeConversationWindows.Find(windoww => windoww.GetChatName() == chatName);
            window.WriteReceivedMessage(username, messageText, messageTime);
        }

        public void LeaveConversation(string chatName)
        {
            IChatWindow window = win.activeConversationWindows.Find(windoww => windoww.GetChatName() == chatName);
            window.LeaveConversation();
        }
    }
}
