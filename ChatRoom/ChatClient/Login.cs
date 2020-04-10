using System;
using System.Collections;
using System.Runtime.Remoting;
using System.Windows.Forms;

namespace ChatClient
{
    public partial class Login : Form
    {
        string port;
        IServerObj server;

        public Login(string port)
        {
            InitializeComponent();

            this.port = port;
        }

        private void Login_Load(object sender, EventArgs e)
        {
            server = (IServerObj)R.New(typeof(IServerObj));     // get reference to the singleton remote object
        }


        private void login_button_Click(object sender, EventArgs e)
        {
            // Make sure the user has filled every field
            if (String.IsNullOrWhiteSpace(username_box.Text) || String.IsNullOrWhiteSpace(password_box.Text))
            {
                MessageBox.Show("Please fill all the boxes.");
                return;
            }


            // Login
            int loginResult = server.Login(username_box.Text, password_box.Text, port);
            if(loginResult == 1)
            {
                server.PerformLogin(username_box.Text, port);
                this.Hide();
                MainWindow mainWindow = new MainWindow(server, username_box.Text, port);
                mainWindow.Show();
            }
            else if(loginResult == 2)
            {
                MessageBox.Show("You are already logged in.");
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


    class R
    {
        private static IDictionary wellKnownTypes;

        public static object New(Type type)
        {
            if (wellKnownTypes == null)
                InitTypeCache();
            WellKnownClientTypeEntry entry = (WellKnownClientTypeEntry)wellKnownTypes[type];
            if (entry == null)
                throw new RemotingException("Type not found!");
            return Activator.GetObject(type, entry.ObjectUrl);
        }

        public static void InitTypeCache()
        {
            Hashtable types = new Hashtable();
            foreach (WellKnownClientTypeEntry entry in RemotingConfiguration.GetRegisteredWellKnownClientTypes())
            {
                if (entry.ObjectType == null)
                    throw new RemotingException("A configured type could not be found!");
                types.Add(entry.ObjectType, entry);
            }
            wellKnownTypes = types;
        }
    }
}
