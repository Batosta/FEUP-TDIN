using System;
using System.Windows.Forms;

namespace Chat
{
    public partial class Register : Form
    {
        readonly IServerObj iServerObj;

        public Register(IServerObj iServerObj)
        {
            this.iServerObj = iServerObj;
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
            iServerObj.Register(username_box.Text, name_box.Text, iServerObj.HashPassword(password_box.Text));

            // Open the login window once again
            Login newLogin = new Login(iServerObj);
            newLogin.Show();
            this.Hide();
        }
    }
}
