﻿using System;
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
        string chatID;

        public ConversationWindow(string chatID, MainWindow win, IServerObj server, string username, string address, string otherUsername)
        {
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
            // Then
            message_viewer.Scrollable = true;
        }

        private void send_message_button_Click(object sender, System.EventArgs e)
        {
            string messageToSend = msg_text_box.Text;
            otherClient.receiveMessage(this.chatID, messageToSend, username, false);
        }

        private void ConversationWindow_Load(object sender, System.EventArgs e)
        {
            otherClient = (IClientObj)RemotingServices.Connect(typeof(IClientObj), address);
        }

        private void domainUpDown1_SelectedItemChanged(object sender, System.EventArgs e)
        {
        }

        public void writeReceivedMessage(string message, string senderUsername, bool isPrivate)
        {
            DateTime localDate = DateTime.Now;
            string time = localDate.ToString("%h:%m:%s");

            window.Invoke((MethodInvoker)delegate { message_viewer.Items.Add(senderUsername + ": " + message + " - " + time); });
        }

        private void message_viewer_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        public string getID()
        {
            return chatID;
        }
    }
}
