using System;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Threading;
using System.Windows.Forms;

namespace ChatClient
{
    public partial class MainWindow : Form
    {

        private static MainWindow mInst;

        ConversationWindow chatwindow;

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
        public List<ConversationWindow> activeConversationWindows;
        RemMessage remMessage;

        public MainWindow(IServerObj server, string username, string port)
        {
            InitializeComponent();
            this.Text = username;       // change the window name to the username for more easy recognition when multiple accounts open on the same computer

            this.server = server;
            this.username = username;
            this.port = port;

            activeConversationWindows = new List<ConversationWindow>();

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
            else
            {
                string selectedUsername = activeSessionsList.SelectedItems[0].Text;
                SendProposal(selectedUsername);
                
                // LATER ALLIGATOR
                //string selectedUsername = activeSessionsList.SelectedItems[0].Text;
                //ConversationWindow newConversationWindow = new ConversationWindow(server, username, port, selectedUsername);
                //activeConversationWindows.Add(newConversationWindow);
                //newConversationWindow.Show();
            }
        }
        
        private void SendProposal(string proposalReceiverUsername)
        {
            ConversationProposal conversationProposal = new ConversationProposal(server, username, proposalReceiverUsername);
            new Thread(() =>
            {
                conversationProposal.SendProposal();
            }).Start();
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

        public delegate void OpeningConversation(ConversationWindow chatwindow);

        private void OpenConversation(ConversationWindow chatwindow)
        {
            if (this.InvokeRequired)
                this.Invoke(new OpeningConversation(OpenConversation), chatwindow);

            else
            {
                chatwindow.Show();
            }
        }

        public void ReceiveYesToProposal(string proposalReceiverUsername, string proposalReceiverAddress)
        {
            ConversationWindow chatwindow = new ConversationWindow(this, server, username, proposalReceiverAddress, proposalReceiverUsername);
            activeConversationWindows.Add(chatwindow);
            OpenConversation(chatwindow);

            // Mensagem a dizer que o outro dude aceitou

        }

        public void ReceiveNoToProposal(string proposalReceiverUsername)
        {
            MessageBox.Show("User " + proposalReceiverUsername + " has declined your conversation.");
            return;
        }

        public void StartAcceptedProposal(string proposalSenderUsername, string proposalSenderAddress)
        {
            ConversationWindow chatwindow = new ConversationWindow(this, server, username, proposalSenderAddress, proposalSenderUsername);
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

        public void ReceiveProposal(string proposalSenderUsername)
        {
            win.ReceiveProposal(proposalSenderUsername);
        }

        public void ReceiveYesToProposal(string proposalReceiverUsername, string proposalReceiverAddress)
        {
            win.ReceiveYesToProposal(proposalReceiverUsername, proposalReceiverAddress);
        }

        public void ReceiveNoToProposal(string proposalReceiverUsername)
        {
            win.ReceiveNoToProposal(proposalReceiverUsername);
        }

        public void StartAcceptedProposal(string proposalSenderUsername, string proposalSenderAddress)
        {
            win.StartAcceptedProposal(proposalSenderUsername, proposalSenderAddress);
        }

        public string test(string test)
        {
            Console.WriteLine("received " + test);
            return "Hi. I received your " + test;
        }

        public void receiveMessage(string message, string senderUsername)
        {
            ConversationWindow window = win.activeConversationWindows.Find(windoww => windoww.otherUsername == senderUsername);
            window.writeReceivedMessage(message, senderUsername);
        }
    }
}
