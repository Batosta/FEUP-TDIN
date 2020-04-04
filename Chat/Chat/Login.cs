using System;
using System.Windows.Forms;

namespace Chat
{
    public partial class Login : Form
    {
        IServerObj iServerObj;

        public Login(IServerObj iServerObj)
        {
            this.iServerObj = iServerObj;
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Open the register window
            Register newRegister = new Register(iServerObj);
            newRegister.Show();
            this.Hide();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click_1(object sender, EventArgs e)
        {

        }
    }
}
