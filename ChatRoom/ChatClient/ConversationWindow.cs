using System;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Windows.Forms;

namespace ChatClient
{
    public partial class ConversationWindow : Form, IChatWindow
    {
        MainWindow window;
        IServerObj server;
        readonly string username;
        readonly string address;
        public readonly string otherUsername;
        IClientObj otherClient;
        readonly string chatID;
        ChatModel chatmodel;
        bool userbyebyed = false;

        public ConversationWindow(string chatID, MainWindow win, IServerObj server, string username, string address, string otherUsername, ChatModel chatmodel)
        {
            this.chatmodel = chatmodel;
            this.chatID = chatID;
            window = win;
            InitializeComponent();

            this.server = server;
            this.username = username;
            this.address = address;
            this.otherUsername = otherUsername;

            string windowText = username + " talking to " + otherUsername;
            this.AcceptButton = send_message_button;

            this.Text = windowText;

            message_viewer.View = View.Details;
            ColumnHeader header = new ColumnHeader();
            message_viewer.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            header.Text = "";
            header.Name = "col1";
            header.Width = message_viewer.Width;
            message_viewer.Columns.Add(header);
            message_viewer.Scrollable = true;

            WriteOldMessages();
        }

        private void WriteOldMessages()
        {
            foreach (MessageModel message in this.chatmodel.Messages)
            {
                if (message.Sender != username)
                {
                    message_viewer.Items.Add(message.Sender + ": " + message.Text + " - " + message.Time);
                }
            }
        }

        private void Send_message_button_Click(object sender, System.EventArgs e)
        {
            DateTime localDate = DateTime.Now;
            string time = localDate.ToString("%h:%m:%s");

            string messageToSend = msg_text_box.Text;
            message_viewer.Items.Add("Me: " + messageToSend + " - " + time);
            otherClient.receiveMessage(this.chatID, messageToSend, time, username, false);
            List<string> receivers = new List<string> { otherUsername };
            server.storeMessage(new MessageModel
            {
                Sender = username,
                Receivers = receivers,
                Text = messageToSend,
                Time = time,
                ChatID = this.chatID
            });
            msg_text_box.Text = "";
        }

        private void ConversationWindow_Load(object sender, System.EventArgs e)
        {
            otherClient = (IClientObj)RemotingServices.Connect(typeof(IClientObj), address);
        }

        public void writeReceivedMessage(string message, string time, string senderUsername, bool isPrivate)
        {
            window.Invoke((MethodInvoker)delegate { message_viewer.Items.Add(senderUsername + ": " + message + " - " + time); });
        }
        private void ConversationWindow_FormClosing(Object sender, FormClosingEventArgs e)
        {
            Console.WriteLine("wtf");
            if(!userbyebyed)
            {
                otherClient.ByeBye(chatID);
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
