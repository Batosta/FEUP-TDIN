using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting;
using System.Threading;
using System.Windows.Forms;

namespace ChatClient
{
    public partial class ConversationWindow : Form
    {
        MainWindow mainWindow;
        IServerObj server;
        readonly string chatName;
        readonly string username;
        byte[] fileToSend;
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
            this.AcceptButton = send_message_button;
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

            string name = CheckPrivate(messageText);
            if (name == "/public")    //  not private
            {
                message_viewer.Items.Add("Me: " + messageText + " - " + messageTime);
                foreach (KeyValuePair<string, IClientObj> entry in otherClients)
                {
                    entry.Value.ReceiveMessage(chatName, username, messageText, messageTime, false);
                }
                server.StoreMessage(chatName, username, messageText, messageTime, false, otherUsernames);
            }
            else if(name != "/nomessage")
            {
                messageText = messageText.Substring(5 + name.Length);
                List<string> receivers = new List<string>();
                if (otherClients.ContainsKey(name))    //  private and valid
                {
                    receivers.Add(name);
                    server.StoreMessage(chatName, username, messageText, messageTime, true, otherUsernames);
                    otherClients[name].ReceiveMessage(chatName, username, messageText, messageTime, true);
                    message_viewer.Items.Add("Me to " + name + ": " + messageText + " - " + messageTime);
                }
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

        string CheckPrivate(string messageToSend)
        {
            if (messageToSend.StartsWith("/pm "))
            {
                int len = messageToSend.Substring(4).IndexOf(' ');
                if (len == -1)
                {
                    return "/nomessage";
                }
                return messageToSend.Substring(4, len);
            }
            else
            {
                return "/public";
            }
        }

        public void WriteReceivedMessage(string username, string messageText, string messageTime, bool isPrivate)
        {
            string messageToBeDisplay = "";
            if (isPrivate)
            {
                messageToBeDisplay += "(pm) " + username + ": " + messageText + " - " + messageTime;
            }
            else
            {
                messageToBeDisplay += username + ": " + messageText + " - " + messageTime;
            }
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
            foreach (MessageModel previousMessage in previousMessages)
            {
                string message = null;
                if (previousMessage.isPrivate)
                {
                    if (previousMessage.Receivers.Contains(username)) // its for me
                        message = "(pm) " + previousMessage.Sender + ": " + previousMessage.Text + " - " + previousMessage.Time;
                    else if (previousMessage.Sender == username)
                        message = "Me to " + previousMessage.Receivers[0] + ": " + previousMessage.Text + " - " + previousMessage.Time;
                }
                else
                {
                    if (previousMessage.Sender.Equals(username))
                        message = "Me: " + previousMessage.Text + " - " + previousMessage.Time;
                    else
                        message = previousMessage.Sender + ": " + previousMessage.Text + " - " + previousMessage.Time;
                }
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

        public void ReceiveFile(byte[] file, string extension)
        {
            //  Testing file received
            Thread t = new Thread((ThreadStart)(() =>
            {
                var folderBrowserDialog1 = new FolderBrowserDialog();

                // Show the FolderBrowserDialog.
                DialogResult result = folderBrowserDialog1.ShowDialog();
                if (result == DialogResult.OK)
                {
                    string folderName = folderBrowserDialog1.SelectedPath;
                    File.WriteAllBytes(folderName + "/test" + extension, file);
                }
            }));
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string extension = Path.GetExtension(openFileDialog1.FileName);
                fileToSend = File.ReadAllBytes(openFileDialog1.FileName);
                Console.WriteLine("length: " + fileToSend.Length);

                foreach (KeyValuePair<string, IClientObj> entry in otherClients)
                {
                    entry.Value.ReceiveFile(chatName,
                                        fileToSend,
                                        extension,
                                        "time",
                                        username,
                                        false);
                }
            }
            
                /*openFileDialog1.Filter =
     "Images (*.BMP;*.JPG;*.GIF,*.PNG,*.TIFF)|*.BMP;*.JPG;*.GIF;*.PNG;*.TIFF";
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    PictureBox PictureBox1 = new PictureBox();
                    PictureBox1.Image = new Bitmap(openFileDialog1.FileName);

                    // Add the new control to its parent's controls collection
                    this.Controls.Add(PictureBox1);
                }*/
            }
    }
}