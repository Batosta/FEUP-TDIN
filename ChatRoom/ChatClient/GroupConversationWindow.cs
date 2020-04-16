using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Windows.Forms;

namespace ChatClient
{
    public partial class GroupConversationWindow : Form, IChatWindow
    {
        MainWindow window;
        IServerObj server;
        readonly string username;
        readonly List<string> addresses;
        public readonly List<string> otherUsernames;
        Dictionary<string, IClientObj> otherClients = new Dictionary<string, IClientObj>();
        string chatID;
        public GroupConversationWindow(string chatID, MainWindow win, IServerObj server, string username, List<string> addresses, List<string> otherUsernames)
        {
            this.chatID = chatID;
            window = win;
            InitializeComponent();

            this.server = server;
            this.username = username;
            this.addresses = addresses;
            this.otherUsernames = otherUsernames;

            //string windowText = username + " talking to " + otherUsername;
            //this.Text = windowText;

            this.AcceptButton = send_message_button;

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
        public void writeReceivedMessage(string message, string senderUsername, bool isPrivate)
        {
            DateTime localDate = DateTime.Now;
            string time = localDate.ToString("%h:%m:%s");

            if (isPrivate)
            {
                window.Invoke((MethodInvoker)delegate { message_viewer.Items.Add("(pm) " + senderUsername + ": " + message + " - " + time); });
            }
            else
            {
                window.Invoke((MethodInvoker)delegate { message_viewer.Items.Add(senderUsername + ": " + message + " - " + time); });
            }
        }

        private void message_viewer_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void send_message_button_Click(object sender, EventArgs e)
        {
            string messageToSend = msg_text_box.Text;
            if(messageToSend == "")
            {
                return;
            }
            string name = checkPrivate(messageToSend);
            if (name == "n")
            {
                foreach (KeyValuePair<string, IClientObj> entry in otherClients)
                {
                    entry.Value.receiveMessage(chatID, messageToSend, username, false);
                }
            }
            else if(name != "c")
            {
                messageToSend = messageToSend.Substring(5 + name.Length);
                if (otherClients.ContainsKey(name))
                {
                    otherClients[name].receiveMessage(chatID, messageToSend, username, true);
                }
            }
            msg_text_box.Text = "";
        }

        private void GroupConversationWindow_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < otherUsernames.Count; i++)
            {
                otherClients.Add(otherUsernames[i], (IClientObj)RemotingServices.Connect(typeof(IClientObj), addresses[i]));
            }
        }

        string checkPrivate(string messageToSend)
        {
            if (messageToSend.StartsWith("/pm "))
            {
                int len = messageToSend.Substring(4).IndexOf(' ');
                if(len == -1)
                {
                    return "c";
                }
                return messageToSend.Substring(4, len);
            }
            else
            {
                return "n";
            }
        }

        public string getID()
        {
            return chatID;
        }
    }
}
