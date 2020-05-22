using System;
using System.Data;
using System.ServiceModel;
using System.Windows.Forms;
using TTService;
using Microsoft.VisualBasic;
using System.Messaging;

namespace TTClient 
{
    public partial class Form1 : Form 
    {
        TTProxy proxy;
        MessageQueue messageQueue;

        public Form1() 
        {
            int k;

            InitializeComponent();

            if (!MessageQueue.Exists(@".\private$\myMSMQ"))
                MessageQueue.Create(@".\private$\myMSMQ");

            proxy = new TTProxy();
            DataTable users = proxy.GetPeopleByRole("solver");
            for (k = 0; k < users.Rows.Count; k++)
                listBox1.Items.Add(users.Rows[k][1]);   // Row 0 is empty; the author name is in column 1

            updateUnassignedTicketsListBox();
        }

        private void listBox1_SelectedIndexChanged(object sender, System.EventArgs e) 
        {
            string solver = listBox1.SelectedItem.ToString();
            DataTable tickets = proxy.GetTicketsBySolver(solver);
            dataGridView2.DataSource = tickets;
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            if(listBox1.SelectedItems.Count != 0)
            {
                if(dataGridView1.SelectedRows.Count != 0)
                {
                    string solver_name = listBox1.SelectedItem.ToString();
                    string ticket_id = dataGridView1.SelectedRows[0].Cells["Id"].Value.ToString();
                    proxy.AssignTicketToSolver(solver_name, ticket_id);
                    updateUnassignedTicketsListBox();
                }
                else
                {
                    MessageBox.Show("Please choose one of the unassigned tickets before assigning it.");
                    return;
                }
            }
            else
            {
                MessageBox.Show("Please choose a solver before assigning a trouble ticket.");
                return;
            }
        }

        private void button2_Click(object sender, System.EventArgs e)
        {
            if(listBox1.SelectedItems.Count != 0)
            {
                if(dataGridView2.SelectedRows.Count != 0)
                {
                    string answer = Interaction.InputBox("Answer to the trouble ticket:", "Answer", "");
                    if (String.IsNullOrWhiteSpace(answer))
                    {
                        MessageBox.Show("Invalid answer.");
                        return;
                    }
                    else
                    {
                        string ticket_id = dataGridView2.SelectedRows[0].Cells["Id"].Value.ToString();
                        proxy.AnswerToTicket(answer, ticket_id);
                    }
                }
                else
                {
                    MessageBox.Show("Please choose one of the assigned tickets before assigning it.");
                    return;
                }
            }
            else
            {
                MessageBox.Show("Please choose a solver before answering a trouble ticket.");
                return;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItems.Count != 0)
            {
                if (dataGridView2.SelectedRows.Count != 0)
                {
                    string questions = Interaction.InputBox("Questions for the department:", "Questions", "");
                    if (String.IsNullOrWhiteSpace(questions))
                    {
                        MessageBox.Show("Invalid input.");
                        return;
                    }
                    else
                    {
                        string ticket_id = dataGridView2.SelectedRows[0].Cells["Id"].Value.ToString();
                        string title = dataGridView2.SelectedRows[0].Cells["Title"].Value.ToString();
                        string problem = dataGridView2.SelectedRows[0].Cells["Problem"].Value.ToString();
                        string[] messageData = new string[4] { ticket_id, title, problem, questions };

                        messageQueue = new MessageQueue(@".\private$\myMSMQ");
                        messageQueue.Formatter = new XmlMessageFormatter(new Type[] { typeof(String[]) });
                        messageQueue.Send(messageData);

                        proxy.TicketWaitingForAnswers(ticket_id);
                    }
                }
                else
                {
                    MessageBox.Show("Please choose one of the assigned tickets sending it to the department.");
                    return;
                }
            }
            else
            {
                MessageBox.Show("Please choose a solver before sending the trouble ticket to the department.");
                return;
            }
        }

        private void updateUnassignedTicketsListBox()
        {
            DataTable unassigned_tickets = proxy.GetUnassignedTickets();
            dataGridView1.DataSource = unassigned_tickets;
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
