using System.Windows.Forms;

namespace ChatClient
{
    public partial class ConversationWindow : Form
    {
        IServerObj server;
        readonly string username;
        readonly string port;
        readonly string otherUsername;

        public ConversationWindow(IServerObj server, string username, string port, string otherUsername)
        {
            InitializeComponent();

            this.server = server;
            this.username = username;
            this.port = port;
            this.otherUsername = otherUsername;

            string windowText = username + " talking to " + otherUsername;
            this.Text = windowText;
        }
    }
}
