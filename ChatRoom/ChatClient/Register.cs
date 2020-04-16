using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChatClient
{
    public partial class Register : Form
    {
        string port;
        IServerObj server;

        public Register(IServerObj server, string port)
        {
            this.server = server;
            this.port = port;

            InitializeComponent();
        }

        private void register_button_Click(object sender, EventArgs e)
        {
            // Make sure the user has filled every field
            if (String.IsNullOrWhiteSpace(username_box.Text) || String.IsNullOrWhiteSpace(name_box.Text) || String.IsNullOrWhiteSpace(password_box.Text))
            {
                MessageBox.Show("Please fill all the boxes.");
                return;
            }

            // No white spaces in username
            if (username_box.Text.Contains(" "))
            {
                MessageBox.Show("Please no white spaces in username.");
                return;
            }


            // Register
            int registerResult = server.Register(username_box.Text, name_box.Text, password_box.Text);
            if(registerResult == -1)
            {
                MessageBox.Show("Username already exists.");
                return;
            }
            else
            {
                server.PerformLogin(username_box.Text, port);
                MessageBox.Show("Successful register.");
                this.Hide();
                MainWindow mainWindow = new MainWindow(server, username_box.Text, port);
                mainWindow.Show();
            }
        }
    }
}
