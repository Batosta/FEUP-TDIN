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
        readonly string chatID;
        bool userbyebyed = false;
        ChatModel chatmodel;
        public GroupConversationWindow(string chatID, MainWindow win, IServerObj server, string username, List<string> addresses, List<string> otherUsernames, ChatModel chatmodel)
        {
            this.chatmodel = chatmodel;
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

            WriteOldMessages();
        }
        private void WriteOldMessages()
        {
            foreach (MessageModel message in this.chatmodel.Messages)
            {
                if(message.Receivers.Count == 1) // its private
                {
                    if (message.Receivers.Contains(username)) // its for me
                    {
                        message_viewer.Items.Add("(pm) " + message.Sender + ": " + message.Text + " - " + message.Time);
                    }
                    else if (message.Sender == username)
                    {
                        message_viewer.Items.Add("Me to " + message.Receivers[0] + ": " + message.Text + " - " + message.Time);
                    }
                }
                else
                {
                    if(message.Sender == username)
                    {
                        message_viewer.Items.Add("Me: " + message.Text + " - " + message.Time);
                    }
                    else
                    {
                        message_viewer.Items.Add(message.Sender + ": " + message.Text + " - " + message.Time);
                    }
                }
            }
        }
        public void writeReceivedMessage(string message, string time, string senderUsername, bool isPrivate)
        {

            if (isPrivate)
            {
                window.Invoke((MethodInvoker)delegate { message_viewer.Items.Add("(pm) " + senderUsername + ": " + message + " - " + time); });
            }
            else
            {
                window.Invoke((MethodInvoker)delegate { message_viewer.Items.Add(senderUsername + ": " + message + " - " + time); });
            }
        }

        private void Message_viewer_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void Send_message_button_Click(object sender, EventArgs e)
        {
            DateTime localDate = DateTime.Now;
            string time = localDate.ToString("%h:%m:%s");

            string messageToSend = msg_text_box.Text;
            if(messageToSend == "")
            {
                return;
            }
            string name = checkPrivate(messageToSend);
            List<string> receivers = new List<string>();
            if (name == "n")    //  not private
            {
                foreach (KeyValuePair<string, IClientObj> entry in otherClients)
                {
                    receivers.Add(entry.Key);
                    message_viewer.Items.Add("Me: " + msg_text_box.Text + " - " + time);
                    entry.Value.receiveMessage(chatID, messageToSend, time, username, false);
                }
                server.storeMessage(new MessageModel
                {
                    Sender = username,
                    Receivers = receivers,
                    Text = messageToSend,
                    Time = time,
                    ChatID = this.chatID
                });
            }
            else if(name != "c")
            {
                messageToSend = messageToSend.Substring(5 + name.Length);
                if (otherClients.ContainsKey(name))    //  private and valid
                {
                    receivers.Add(name);
                    server.storeMessage(new MessageModel
                    {
                        Sender = username,
                        Receivers = receivers,
                        Text = messageToSend,
                        Time = time,
                        ChatID = this.chatID
                    });
                    otherClients[name].receiveMessage(chatID, messageToSend, time, username, true);
                    message_viewer.Items.Add("Me to " + name + ": " + msg_text_box.Text + " - " + time);
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
        private void GroupConversationWindow_FormClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Console.WriteLine("wtf");
            if (!userbyebyed)
            {
                foreach (KeyValuePair<string, IClientObj> entry in otherClients)
                {
                    entry.Value.ByeBye(chatID);
                }
                window.activeConversationWindows.Remove(this);
            }
            else
            {
                window.activeConversationWindows.Remove(this);
            }
        }

        public void userByebyed()
        {
            userbyebyed = true;
            window.Invoke((MethodInvoker)delegate { this.Close(); });
        }

        public string getID()
        {
            return chatID;
        }
    }
}
