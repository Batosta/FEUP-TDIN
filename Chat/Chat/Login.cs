using System;
using System.Windows.Forms;

namespace ChatClient
{
    public partial class Login : Form
    {
        readonly IServerObj server;
        readonly string port;

        public Login(IServerObj server, string port)
        {
            this.server = server;
            this.port = port;

            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Open the register window
            Register newRegister = new Register(server, port);
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

            // Make sure the user has filled every field
            if (String.IsNullOrWhiteSpace(username_box.Text) || String.IsNullOrWhiteSpace(password_box.Text))
            {
                MessageBox.Show("Please fill all the boxes.");
                return;
            }


            int loginResult = server.Login(username_box.Text, password_box.Text, port);
            if (loginResult == 1)
            {
                server.PerformLogin(username_box.Text, port);
                this.Hide();
                MainWindow newMainwindow = new MainWindow(server, username_box.Text, port);
                newMainwindow.Show();
            }
            else if (loginResult == 2)
            {
                MessageBox.Show("You are already logged in");
            }
            else if (loginResult == 3)
            {
                MessageBox.Show("The password for that username is not correct.");
            }
            else
            {
                MessageBox.Show("There is no user with that username.");
            }
        }
    }
}
