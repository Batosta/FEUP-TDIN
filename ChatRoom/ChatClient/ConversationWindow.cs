using System;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Windows.Forms;

namespace ChatClient
{
    public partial class ConversationWindow : Form, IChatWindow
    {
        MainWindow mainWindow;
        IServerObj server;
        readonly string chatName;
        readonly string username;
        readonly List<string> otherUsernames;
        readonly List<string> otherUsersAddresses;
        Dictionary<string, IClientObj> otherClients = new Dictionary<string, IClientObj>();

        bool userLeft = false;

        public ConversationWindow(MainWindow win, IServerObj server, string chatName, string username, List<string> otherUsernames, List<string> otherUsersAddresses, List<MessageModel> previousMessages)
        {
            this.mainWindow = win;

            InitializeComponent();

            this.server = server;
            this.chatName = chatName;
            this.username = username;
            this.otherUsernames = otherUsernames;
            this.otherUsersAddresses = otherUsersAddresses;

            string windowText = GetConversationTitle();
            this.Text = windowText;

            this.AcceptButton = send_message_button;

            SetupMessageViewer();
            if(previousMessages != null)
                WritePreviousMessages(previousMessages);
        }

        private void ConversationWindow_Load(object sender, System.EventArgs e)
        {
            for(int i = 0; i < otherUsernames.Count; i++)
            {
                IClientObj newClient = (IClientObj)RemotingServices.Connect(typeof(IClientObj), otherUsersAddresses[i]);
                otherClients.Add(otherUsernames[i], newClient);
            }
        }

        private void Send_message_button_Click(object sender, System.EventArgs e)
        {
            string messageTime = DateTime.Now.ToString("%h:%m:%s");

            // Check if there is a valid text for the message
            if (String.IsNullOrWhiteSpace(msg_text_box.Text))
            {
                return;
            }
            string messageText = msg_text_box.Text;

            message_viewer.Items.Add("Me: " + messageText + " - " + messageTime);
            server.StoreMessage(chatName, username, messageText, messageTime, otherUsernames);

            foreach (KeyValuePair<string, IClientObj> entry in otherClients)
            {
                entry.Value.ReceiveMessage(chatName, username, messageText, messageTime);
            }

            msg_text_box.Text = "";
        }
        
        private void ConversationWindow_FormClosing(Object sender, FormClosingEventArgs e)
        {
            if (!userLeft)
            {
                foreach(KeyValuePair<string, IClientObj> entry in otherClients)
                {
                    entry.Value.LeaveConversation(chatName);
                }
            }
            mainWindow.activeConversationWindows.Remove(this);
        }

        public void WriteReceivedMessage(string username, string messageText, string messageTime)
        {
            string messageToBeDisplay = username + ": " + messageText + " - " + messageTime;
            mainWindow.Invoke((MethodInvoker)delegate
            {
                message_viewer.Items.Add(messageToBeDisplay);
            });
        }
        
        public string GetChatName()
        {
            return chatName;
        }
        
        public List<string> GetOtherUsernames()
        {
            return otherUsernames;
        }

        public void LeaveConversation()
        {
            userLeft = true;
            mainWindow.Invoke((MethodInvoker)delegate { this.Close(); });
        }

        // Writes all the messages previously sent in another connections between this group of users
        private void WritePreviousMessages(List<MessageModel> previousMessages)
        {
            foreach(MessageModel previousMessage in previousMessages)
            {
                string message = null;
                if (previousMessage.Sender.Equals(username))
                    message = "Me: " + previousMessage.Text + " - " + previousMessage.Time;
                else
                    message = previousMessage.Sender + ": " + previousMessage.Text + " - " + previousMessage.Time;

                message_viewer.Items.Add(message);
            }
        }

        private void SetupMessageViewer()
        {
            message_viewer.View = View.Details;
            ColumnHeader header = new ColumnHeader();
            message_viewer.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            header.Text = "";
            header.Name = "col1";
            header.Width = message_viewer.Width;
            message_viewer.Columns.Add(header);
            // Then
            message_viewer.Scrollable = true;

            string stringUsernames = otherUsernames[0];
            for (int i = 1; i < otherUsernames.Count; i++)
            {
                stringUsernames += ", " + otherUsernames[i];
            }

            message_viewer.Items.Add("Chatting with " + stringUsernames);
        }

        private string GetConversationTitle()
        {
            string windowText = username + " talking to ";
            foreach(string username in otherUsernames)
            {
                windowText += username + ", ";
            }
            return windowText.Remove(windowText.Length - 2);
        }
    }
}