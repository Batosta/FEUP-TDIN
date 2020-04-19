using SharpCompress.Common;
using System;
using System.Collections.Generic;
using System.IO;
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
        public Dictionary<string, ConversationWindow> activeConversationWindows;
        RemMessage remMessage;

        public MainWindow(IServerObj server, string username, string port)
        {
            InitializeComponent();
            this.Text = username;       // change the window name to the username for more easy recognition when multiple accounts open on the same computer

            this.server = server;
            this.username = username;
            this.port = port;

            activeConversationWindows = new Dictionary<string, ConversationWindow>();

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

                    lvRem = new LVRemDelegate(RemoveLoggedOutUser);
                    BeginInvoke(lvRem, new object[] { username });
                    break;

                default:
                    break;
            }
        }

        private void RemoveLoggedOutUser(string loggedOutUsername)
        {
            foreach (ListViewItem item in activeSessionsList.Items)
            {
                if (item.SubItems[0].Text.Equals(loggedOutUsername))
                {
                    item.Remove();
                    break;
                }
            }

            for (int i = 0; i < activeSessions.Count; i++)
            {
                if (activeSessions[i].username.Equals(loggedOutUsername))
                {
                    activeSessions.RemoveAt(i);
                    break;
                }
            }
            List<string> conversationsToLeave = new List<string>();
            foreach (KeyValuePair<string, ConversationWindow> entry in activeConversationWindows)
            {
                if (entry.Value.GetOtherUsernames().Contains(loggedOutUsername))
                {
                    conversationsToLeave.Add(entry.Key);
                    //activeConversationWindows.Remove(activeConversationWindows.ElementAt(i).Key);
                }
            }
            
            foreach ( string conversationToLeave in conversationsToLeave)
            {
                activeConversationWindows[conversationToLeave].LeaveConversation();
            }
            /*
            for (int i = 0; i < activeConversationWindows.Count; i++)
            {
                if (activeConversationWindows.ElementAt(i).Value.GetOtherUsernames().Contains(loggedOutUsername))
                {
                    activeConversationWindows.ElementAt(i).Value.LeaveConversation();
                    //activeConversationWindows.Remove(activeConversationWindows.ElementAt(i).Key);
                    i = 0;
                }
            }
            */
        }

        private void start_conversation_Click(object sender, EventArgs e)
        {
            if (activeSessionsList.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please choose at least one user to chat with.");
                return;
            }
            else
            {
                string chatname = username;
                List<string> selectUsernames = new List<string>();
                foreach (ListViewItem selectedUsername in activeSessionsList.SelectedItems)
                {
                    chatname += selectedUsername.Text;
                    selectUsernames.Add(selectedUsername.Text);
                }
                if (!activeConversationWindows.ContainsKey(String.Concat(chatname.OrderBy(c => c))))
                    SendProposal(selectUsernames);
                else
                    MessageBox.Show("You area already chatting with this user(s)");
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

        public void StartChat(string chatName, List<string> usernames, List<string> addresses, List<MessageModel> previousMessages)
        {
            for (int i = 0; i < usernames.Count; i++)
            {
                if (usernames[i].Equals(username))
                {
                    usernames.RemoveAt(i);
                    addresses.RemoveAt(i);
                    break;
                }
            }

            ConversationWindow conversationWindow = new ConversationWindow(this, server, chatName, username, usernames, addresses, previousMessages);
            activeConversationWindows[chatName] = conversationWindow;
            this.Invoke((MethodInvoker)delegate
            {
                conversationWindow.Show();
            });
        }


        // Builds the proposal message for both simple and group chats
        private string BuildConversationProposalMessage(string proposalSenderUsername, List<string> proposalReceiverUsernames)
        {
            string message = "";
            if (proposalReceiverUsernames.Count == 1)
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

        private void logout_button_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void MainWindow_FormClosing(Object sender, FormClosingEventArgs e)
        {
            List<string> conversationsToLeave = new List<string>();
            foreach (KeyValuePair<string, ConversationWindow> entry in activeConversationWindows)
            {
                conversationsToLeave.Add(entry.Key);
            }

            foreach (string conversationToLeave in conversationsToLeave)
            {
                activeConversationWindows[conversationToLeave].LeaveConversation();
            }
            server.alterEvent -= new AlterDelegate(evRepeater.Repeater);
            evRepeater.alterEvent -= new AlterDelegate(DoAlterations);
            server.Logout(username);
            if (System.Windows.Forms.Application.MessageLoop)
            {
                // WinForms app
                System.Windows.Forms.Application.Exit();
            }
            else
            {
                // Console app
                System.Environment.Exit(1);
            }
        }

        private void help_button_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Select the user you want to chat with and then press the Start Conversation button." +
                " If you pretend to have a group chat, select more than 1 user before clicking the Start Conversation.");
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

        public void ReceiveMessage(string chatName, string username, string messageText, string messageTime, bool isPrivate)
        {
            win.activeConversationWindows[chatName].WriteReceivedMessage(username, messageText, messageTime, isPrivate);
        }

        public void LeaveConversation(string chatName)
        {
            win.activeConversationWindows[chatName].LeaveConversation();
        }

        public void ReceiveFile(string chatName, string username, byte[] file, string extension, string messageTime)
        {
            win.activeConversationWindows[chatName].ReceiveFile(username, file, extension, messageTime);
        }

        public void SetupConnection()
        {
        }
    }
}
