using Microsoft.VisualBasic;
using System;
using System.Data;
using System.Messaging;
using System.ServiceModel;
using System.Windows.Forms;
using TTService;

namespace TTDepartment
{
    public partial class Form1 : Form
    {
        TTProxy proxy;
        public Form1()
        {
            proxy = new TTProxy();

            InitializeComponent();

            ListenToMessageQueue();
        }

        public void ListenToMessageQueue()
        {
            MessageQueue messageQueue = new MessageQueue();
            messageQueue.Path = @".\private$\myMSMQ";
            messageQueue.ReceiveCompleted += HandleReceivedMessage;
            messageQueue.BeginReceive();
        }

        public void HandleReceivedMessage(object obj, ReceiveCompletedEventArgs args)
        {
            if (InvokeRequired)
            {
                Invoke((MethodInvoker)delegate
                {
                    HandleReceivedMessage(obj, args);
                });
            }
            else
            {
                MessageQueue msgQueue = (MessageQueue)obj;
                System.Messaging.Message newMessage = null;

                newMessage = msgQueue.EndReceive(args.AsyncResult);
                newMessage.Formatter = new XmlMessageFormatter(new Type[] { typeof(String[]) });

                String[] messageData = (String[])newMessage.Body;

                UpdateUnansweredTickets(messageData);

                msgQueue.BeginReceive();
            }
        }

        private void UpdateUnansweredTickets(String[] messageData)
        {
            dataGridView1.Rows.Add(messageData[0], messageData[1], messageData[2], messageData[3]);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count != 0)
            {
                string answer = Interaction.InputBox("Answer:", "Answers", "");
                if (String.IsNullOrWhiteSpace(answer))
                {
                    MessageBox.Show("Invalid input.");
                    return;
                }
                else
                {
                    string ticket_id = dataGridView1.SelectedRows[0].Cells["Id"].Value.ToString();
                    dataGridView1.Rows.RemoveAt(dataGridView1.SelectedRows[0].Index);
                    proxy.AnswerToQuestion(answer, ticket_id);
                }
            }
            else
            {
                MessageBox.Show("Please choose one of the questions.");
                return;
            }
        }
    }

    // Manual proxy to the service (in alternative to direct HTTP requests)
    class TTProxy : ClientBase<ITTService>, ITTService
    {
        public int AddTicket(string author, string title, string problem)
        {
            return Channel.AddTicket(author, title, problem);
        }

        public void AssignTicketToSolver(string solver_name, string ticket_id)
        {
            Channel.AssignTicketToSolver(solver_name, ticket_id);
        }

        public void AnswerToTicket(string answer, string ticket_id)
        {
            Channel.AnswerToTicket(answer, ticket_id);
        }

        public void AnswerToQuestion(string answer, string ticket_id)
        {
            Channel.AnswerToQuestion(answer, ticket_id);
        }

        public void TicketWaitingForAnswers(string ticket_id)
        {
            Channel.TicketWaitingForAnswers(ticket_id);
        }

        public DataTable GetUnassignedTickets()
        {
            return Channel.GetUnassignedTickets();
        }

        public DataTable GetTicketsByAuthor(string author)
        {
            return Channel.GetTicketsByAuthor(author);
        }

        public DataTable GetTicketsBySolver(string solver)
        {
            return Channel.GetTicketsBySolver(solver);
        }

        public DataTable GetPeopleByRole(string role)
        {
            return Channel.GetPeopleByRole(role);
        }
    }
}
