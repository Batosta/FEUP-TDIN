using System;
using System.Windows.Forms;

namespace ChatClient
{
    public partial class Register : Form
    {
        readonly IServerObj server;
        readonly string port;

        public Register(IServerObj server, string port)
        {
            this.server = server;
            this.port = port;

            InitializeComponent();
        }

        private void username_label_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void register_button_Click(object sender, EventArgs e)
        {
            // Make sure the user has filled every field
            if (String.IsNullOrWhiteSpace(username_box.Text) || String.IsNullOrWhiteSpace(name_box.Text) || String.IsNullOrWhiteSpace(password_box.Text))
            {
                MessageBox.Show("Please fill all the boxes.");
                return;
            }

            // Register the user in the database
            int registerResult = server.Register(username_box.Text, name_box.Text, server.HashPassword(password_box.Text));
            if (registerResult == -1)
            {
                MessageBox.Show("Username already exists.");
                return;
            }

            // Open the login window once again
            Login newLogin = new Login(server, port);
            newLogin.Show();
            this.Hide();
        }

        private void Register_Load(object sender, EventArgs e)
        {

        }
    }
}
