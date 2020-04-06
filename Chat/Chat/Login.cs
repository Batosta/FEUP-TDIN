using System;
using System.Windows.Forms;

namespace Chat
{
    public partial class Login : Form
    {
        readonly IServerObj serverObj;

        public Login(IServerObj serverObj)
        {
            this.serverObj = serverObj;
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Open the register window
            Register newRegister = new Register(serverObj);
            newRegister.Show();
            this.Hide();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click_1(object sender, EventArgs e)
        {

        }

        private void login_button_Click(object sender, EventArgs e)
        {   
            //  TODO User will have to send its address and port

            if (String.IsNullOrWhiteSpace(username_box.Text) || String.IsNullOrWhiteSpace(password_box.Text))
            {
                MessageBox.Show("Please fill all the boxes.");
                return;
            }
            int loginResult = serverObj.Login(username_box.Text, password_box.Text, Chat.host, Chat.port);
            switch (loginResult)
            {
                case 1:
                    //MessageBox.Show("Correct username and password.");
                    break;

                case 2:
                    MessageBox.Show("The password for that username is not correct.");
                    return;

                case 3:
                    MessageBox.Show("There is no user with that username.");
                    return;

                default:
                    break;
            }

            Chat.username = username_box.Text;

            // Open the MainWindow
            MainWindow newMainwindow = new MainWindow(serverObj);
            newMainwindow.Show();
            this.Hide();

        }
    }
}
