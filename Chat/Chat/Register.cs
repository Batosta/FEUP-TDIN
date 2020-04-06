using System;
using System.Windows.Forms;

namespace Chat
{
    public partial class Register : Form
    {
        readonly IServerObj serverObj;

        public Register(IServerObj serverObj)
        {
            this.serverObj = serverObj;
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
            // Register the user in the database
            if (String.IsNullOrWhiteSpace(username_box.Text) || String.IsNullOrWhiteSpace(name_box.Text) || String.IsNullOrWhiteSpace(password_box.Text))
            {
                MessageBox.Show("Please fill all the boxes.");
                return;
            }

            int registerResult = serverObj.Register(username_box.Text, name_box.Text, serverObj.HashPassword(password_box.Text));
            if (registerResult == -1)
            {
                MessageBox.Show("Username already exists.");
                return;
            }

            // Open the login window once again
            Login newLogin = new Login(serverObj);
            newLogin.Show();
            this.Hide();
        }

        private void Register_Load(object sender, EventArgs e)
        {

        }
    }
}
