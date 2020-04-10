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
    public partial class MainWindow : Form
    {
        // server + port
        IServerObj server;
        readonly string username;
        readonly string port;

        // delegates + events
        delegate ListViewItem LVAddDelegate(ListViewItem lvItem);
        delegate void LVRemDelegate(string lvItem);
        AlterEventRepeater evRepeater;

        // other variables
        List<UserSession> activeSessions;


        public MainWindow(IServerObj server, string username, string port)
        {
            InitializeComponent();

            this.server = server;
            this.username = username;
            this.port = port;

            PlaceActiveSessions();
            AlterEventRepeaterSection();
        }

        private void AlterEventRepeaterSection()
        {
            evRepeater = new AlterEventRepeater();
            evRepeater.alterEvent += new AlterDelegate(DoAlterations);
            server.alterEvent += new AlterDelegate(evRepeater.Repeater);
        }

        private void PlaceActiveSessions()
        {
            this.activeSessions = server.GetActiveSessions();
            activeSessions = server.GetActiveSessions();
            foreach(UserSession activeSession in activeSessions)
            {
                if(activeSession.username != username)
                {
                    activeSessionsList.Items.Add(activeSession.username);
                }
            }
        }



        public void DoAlterations(Operation op, string username, string port)
        {
            LVAddDelegate lvAdd;
            LVRemDelegate lvRem;

            switch (op)
            {
                case Operation.SessionStart:

                    UserSession newUserSession = new UserSession(username, port);
                    activeSessions.Add(newUserSession);
                    lvAdd = new LVAddDelegate(activeSessionsList.Items.Add);
                    ListViewItem lvItem = new ListViewItem(new string[] { username });
                    // Checkar esta parte
                    BeginInvoke(lvAdd, new object[] { lvItem });
                    //lvAdd.Invoke(lvItem);       // como o stor tem
                    break;

                case Operation.SessionEnd:
                    Console.WriteLine("DoAlterations for Operation.SessionEnd");
                    break;

                default:
                    break;
            }
        }
    }




    public class RemMessage : MarshalByRefObject, IClientObj
    {
        private MainWindow win;

        public override object InitializeLifetimeService()
        {
            return null;
        }

        public void PutMyForm(MainWindow form)
        {
            win = form;
        }
    }
}
