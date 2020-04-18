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

        private static MainWindow mInst;

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
        //public List<GroupConversationWindow> activeGroupConversationWindows;
        RemMessage remMessage;

        public MainWindow(IServerObj server, string username, string port)
        {
            InitializeComponent();
            this.Text = username;       // change the window name to the username for more easy recognition when multiple accounts open on the same computer

            this.server = server;
            this.username = username;
            this.port = port;

            activeConversationWindows = new List<IChatWindow>();
            //activeGroupConversationWindows = new List<GroupConversationWindow>();

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
        public static MainWindow CheckInst
        {
            get
            {
                return mInst;
            }
        }

        public static MainWindow CreateInst(IServerObj server, string username, string port)
        {
            if (mInst == null)
                mInst = new MainWindow(server, username, port);
            return mInst;
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
            if(activeSessionsList.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please choose a user to chat with.");
                return;
            }
            else if(activeSessionsList.SelectedItems.Count == 1)
            {
                string selectedUsername = activeSessionsList.SelectedItems[0].Text;
                SendProposal(selectedUsername);
                
                // LATER ALLIGATOR
                //string selectedUsername = activeSessionsList.SelectedItems[0].Text;
                //ConversationWindow newConversationWindow = new ConversationWindow(server, username, port, selectedUsername);
                //activeConversationWindows.Add(newConversationWindow);
                //newConversationWindow.Show();
            }
            else
            {
                List<string> selectedUsernames = new List<string>();
                foreach (ListViewItem selectedUsername in activeSessionsList.SelectedItems)
                {
                    selectedUsernames.Add(selectedUsername.Text);
                }
                SendGroupProposal(selectedUsernames);
            }
        }

        private void SendGroupProposal(List<string> proposalReceiverUsernames)
        {
            new Thread(() =>
            {
                server.SendGroupProposal(username, proposalReceiverUsernames);
            }).Start();
        }

        private void SendProposal(string proposalReceiverUsername)
        {
            ConversationProposal conversationProposal = new ConversationProposal(server, username, proposalReceiverUsername);
            new Thread(() =>
            {
                conversationProposal.SendProposal();
            }).Start();
        }

        public void ReceiveGroupProposal(string proposalSenderUsername, List<string> porposalReceiverUsernames)
        {
            string others = "";
            foreach(string other in porposalReceiverUsernames)
            {
                if(other != username)
                {
                    others += (other + ", ");
                }
            }
            others = others.Remove(others.Length - 2, 2);

            string message = proposalSenderUsername + " is proposing a group conversation with " + others + ". Do you also want to chat with him/her?";
            string caption = "Conversation Group Proposal";
            DialogResult proposalResult = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (proposalResult == DialogResult.Yes)       // If the proposal receiver answers Yes
                server.YesToGroupProposal(proposalSenderUsername, username);
            else                                         // If the proposal receiver answers No
                server.NoToProposal(proposalSenderUsername, username);
        }

        public void ReceiveProposal(string proposalSenderUsername)
        {
            string message = proposalSenderUsername + " is proposing a conversation. Do you also want to chat with him/her?";
            string caption = "Conversation Proposal";
            DialogResult proposalResult = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            
            if(proposalResult == DialogResult.Yes)       // If the proposal receiver answers Yes
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

        public void ReceiveYesToProposal(string proposalReceiverUsername, string proposalReceiverAddress, string chatID, ChatModel chatmodel)
        {
            ConversationWindow chatwindow = new ConversationWindow(chatID, this, server, username, proposalReceiverAddress, proposalReceiverUsername, chatmodel);
            activeConversationWindows.Add(chatwindow);
            OpenConversation(chatwindow);

            // Mensagem a dizer que o outro dude aceitou

        }

        internal void StartGroupChat(List<string> usernames, List<string> addresses, string chatID, ChatModel chatmodel)
        {
            for (int i = 0; i < usernames.Count; i++)
            {
                if (usernames[i] == username)
                {
                    usernames.RemoveAt(i);
                    addresses.RemoveAt(i);
                }
            }
            GroupConversationWindow chatwindow = new GroupConversationWindow(chatID, this, server, username, addresses, usernames, chatmodel);
            activeConversationWindows.Add(chatwindow);
            OpenConversation(chatwindow);
        }

        public void ReceiveNoToProposal(string proposalReceiverUsername)
        {
            MessageBox.Show("User " + proposalReceiverUsername + " has declined your conversation.");
            return;
        }

        public void StartAcceptedProposal(string proposalSenderUsername, string proposalSenderAddress, string chatID, ChatModel chatmodel)
        {   
            ConversationWindow chatwindow = new ConversationWindow(chatID, this, server, username, proposalSenderAddress, proposalSenderUsername, chatmodel);
            activeConversationWindows.Add(chatwindow);
            OpenConversation(chatwindow);
            // Mensagem a dizer que ja vai começar a conversa
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            IsMdiContainer = true;

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

        public void ReceiveGroupProposal(string proposalSenderUsername, List<string> porposalReceiverUsernames)
        {
            win.ReceiveGroupProposal(proposalSenderUsername, porposalReceiverUsernames);
        }

        public void ReceiveProposal(string proposalSenderUsername)
        {
            win.ReceiveProposal(proposalSenderUsername);
        }

        public void ReceiveYesToProposal(string proposalReceiverUsername, string proposalReceiverAddress, string chatID, ChatModel chatmodel)
        {
            win.ReceiveYesToProposal(proposalReceiverUsername, proposalReceiverAddress, chatID, chatmodel);
        }
        
        public void StartGroupChat(List<string> usernames, List<string> addresses, string chatID, ChatModel chatmodel)
        {
            win.StartGroupChat(usernames, addresses, chatID, chatmodel);
        }

        public void ReceiveNoToProposal(string proposalReceiverUsername)
        {
            win.ReceiveNoToProposal(proposalReceiverUsername);
        }

        public void StartAcceptedProposal(string proposalSenderUsername, string proposalSenderAddress, string chatID, ChatModel chatmodel)
        {
            win.StartAcceptedProposal(proposalSenderUsername, proposalSenderAddress, chatID, chatmodel);
        }

        public string test(string test)
        {
            Console.WriteLine("received " + test);
            return "Hi. I received your " + test;
        }

        public void receiveMessage(string chatID, string message, string time, string senderUsername, bool isPrivate)
        {
            IChatWindow window = win.activeConversationWindows.Find(windoww => windoww.getID() == chatID);
            window.writeReceivedMessage(message, time, senderUsername, isPrivate);
        }

        public void ByeBye(string chatID)
        {
            IChatWindow window = win.activeConversationWindows.Find(windoww => windoww.getID() == chatID);
            window.userByebyed();
        }
    }
}
