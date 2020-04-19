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
        byte[] fileToSend;

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

        public void receiveFile(byte[] file, string extension)
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
                otherClient.receiveFile(chatID,
                                        fileToSend,
                                        extension,
                                        "time",
                                        username,
                                        false);
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
