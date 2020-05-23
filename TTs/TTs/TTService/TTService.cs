using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.Net.Mail;

namespace TTService
{
    public class TTService : ITTService
    {
        readonly string database;

        TTService()
        {
            string connection = ConfigurationManager.ConnectionStrings["TTs"].ConnectionString;
            database = String.Format(connection, AppDomain.CurrentDomain.BaseDirectory);
        }

        public int AddTicket(string author, string title, string problem)
        {
            int id = 0;

            using (SqlConnection c = new SqlConnection(database))
            {
                try
                {
                    c.Open();
                    string sql = "insert into TroubleTickets(Author, Title, Problem, Status, Solver, Answer, DepartmentExplanation, Date) values (@a1, @t1, @p1, 'unassigned', NULL, NULL, NULL, @d1)";     // injection protection
                    SqlCommand cmd = new SqlCommand(sql, c);                                                                                        // injection protection
                    cmd.Parameters.AddWithValue("@a1", author);                                                                                     // injection protection
                    cmd.Parameters.AddWithValue("@t1", title);                                                                                      // injection protection
                    cmd.Parameters.AddWithValue("@p1", problem);                                                                                    // injection protection
                    cmd.Parameters.AddWithValue("@d1", GetDate());                                                                                  // injection protection
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "select max(Id) from TroubleTickets";
                    id = (int)cmd.ExecuteScalar();
                }
                catch (SqlException sqlEx)
                {
                    Console.WriteLine(sqlEx);
                }
                finally
                {
                    c.Close();
                }
            }
            return id;
        }

        public void AssignTicketToSolver(string solver_name, string ticket_id)
        {
            using (SqlConnection c = new SqlConnection(database))
            {
                try
                {
                    c.Open();
                    string sql = "update TroubleTickets set Status=@s1, Solver=@s2 where Id=@i1";
                    SqlCommand cmd = new SqlCommand(sql, c);
                    cmd.Parameters.AddWithValue("@s1", "assigned");
                    cmd.Parameters.AddWithValue("@s2", solver_name);
                    cmd.Parameters.AddWithValue("@i1", Int32.Parse(ticket_id));
                    cmd.ExecuteNonQuery();
                }
                catch (SqlException sqlEx)
                {
                    Console.WriteLine(sqlEx);
                }
                finally
                {
                    c.Close();
                }
            }
        }

        public void AnswerToTicket(string answer, string ticket_id)
        {
            using (SqlConnection c = new SqlConnection(database))
            {
                try
                {
                    c.Open();
                    string sql = "update TroubleTickets set Answer=@a1, Status='solved' where Id=@i1";
                    SqlCommand cmd = new SqlCommand(sql, c);
                    cmd.Parameters.AddWithValue("@a1", answer);
                    cmd.Parameters.AddWithValue("@i1", Int32.Parse(ticket_id));
                    cmd.ExecuteNonQuery();

                    SendEmail(ticket_id);
                }
                catch (SqlException sqlEx)
                {
                    Console.WriteLine(sqlEx);
                }
                finally
                {
                    c.Close();
                }
            }
        }

        private void SendEmail(string ticket_id)
        {
            DataTable ticket = GetTicketById(ticket_id);
            DataTable person = GetPersonById(ticket.Rows[0]["Author"].ToString());
            string email = person.Rows[0]["Email"].ToString();



            //cria uma mensagem
            MailMessage mail = new MailMessage();

            //define os endereços
            mail.From = new MailAddress("solverproblem05@gmail.com");
            mail.To.Add(email);

            //define o conteúdo
            mail.Subject = "Problem " + ticket.Rows[0]["Title"].ToString() + " solved";

            string body = "Title: " + ticket.Rows[0]["Title"].ToString() + "\n\n";
            body += "Problem: " + ticket.Rows[0]["Problem"].ToString() + "\n\n";
            body += "Answer: " + ticket.Rows[0]["Answer"].ToString();
            mail.Body = body;

            try
            {
                //envia a mensagem
                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                smtp.EnableSsl = true;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.UseDefaultCredentials = false;
                NetworkCredential cred = new NetworkCredential("solverproblem05@gmail.com", "problemsolver");
                // inclui as credenciais
                smtp.Credentials = cred;
                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public void AnswerToQuestion(string answer, string ticket_id)
        {
            using (SqlConnection c = new SqlConnection(database))
            {
                try
                {
                    c.Open();
                    string sql = "update TroubleTickets set DepartmentExplanation=@a1, Status='assigned' where Id=@i1";
                    SqlCommand cmd = new SqlCommand(sql, c);
                    cmd.Parameters.AddWithValue("@a1", answer);
                    cmd.Parameters.AddWithValue("@i1", Int32.Parse(ticket_id));
                    cmd.ExecuteNonQuery();
                }
                catch (SqlException sqlEx)
                {
                    Console.WriteLine(sqlEx);
                }
                finally
                {
                    c.Close();
                }
            }
        }
        
        public void TicketWaitingForAnswers(string ticket_id)
        {
            using (SqlConnection c = new SqlConnection(database))
            {
                try
                {
                    c.Open();
                    string sql = "update TroubleTickets set Status='waiting for answers' where Id=@i1";
                    SqlCommand cmd = new SqlCommand(sql, c);
                    cmd.Parameters.AddWithValue("@i1", Int32.Parse(ticket_id));
                    cmd.ExecuteNonQuery();
                }
                catch (SqlException sqlEx)
                {
                    Console.WriteLine(sqlEx);
                }
                finally
                {
                    c.Close();
                }
            }
        }

        public DataTable GetUnassignedTickets()
        {
            DataTable result = new DataTable("UnassignedTickets");

            using (SqlConnection c = new SqlConnection(database))
            {
                try
                {
                    c.Open();
                    string sql = "select Id, Title, Problem, Date from TroubleTickets where Status='unassigned'";
                    SqlCommand cmd = new SqlCommand(sql, c);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(result);
                }
                catch (SqlException sqlEx)
                {
                    Console.WriteLine(sqlEx);
                }
                finally
                {
                    c.Close();
                }
            }
            return result;
        }

        public DataTable GetTicketsByAuthor(string author)
        {
            DataTable result = new DataTable("TroubleTickets");

            using (SqlConnection c = new SqlConnection(database))
            {
                try
                {
                    c.Open();
                    string sql = "select Title, Problem, Status, Solver, Answer, Date from TroubleTickets where Author=@a1";
                    SqlCommand cmd = new SqlCommand(sql, c);
                    cmd.Parameters.AddWithValue("@a1", author);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(result);
                }
                catch (SqlException sqlEx)
                {
                    Console.WriteLine(sqlEx);
                }
                finally
                {
                    c.Close();
                }
            }

            return result;
        }

        public DataTable GetTicketsBySolver(string solver)
        {
            DataTable result = new DataTable("Tickets");

            using (SqlConnection c = new SqlConnection(database))
            {
                try
                {
                    c.Open();
                    string sql = "select Id, Title, Problem, Status, DepartmentExplanation, Date from TroubleTickets where (Status=@s1 OR Status=@s2) AND Solver=@s4";
                    SqlCommand cmd = new SqlCommand(sql, c);
                    cmd.Parameters.AddWithValue("@s1", "assigned");
                    cmd.Parameters.AddWithValue("@s2", "waiting for answers");
                    cmd.Parameters.AddWithValue("@s4", solver);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(result);
                }
                catch (SqlException sqlEx)
                {
                    Console.WriteLine(sqlEx);
                }
                finally
                {
                    c.Close();
                }
            }
            return result;
        }

        public DataTable GetPeopleByRole(string role)
        {
            DataTable result = new DataTable("People");

            using (SqlConnection c = new SqlConnection(database))
            {
                try
                {
                    c.Open();
                    string sql = "select * from People where Role=@r1";
                    SqlCommand cmd = new SqlCommand(sql, c);
                    cmd.Parameters.AddWithValue("@r1", role);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(result);
                }
                catch (SqlException sqlEx)
                {
                    Console.WriteLine(sqlEx);
                }
                finally
                {
                    c.Close();
                }
            }

            return result;
        }



        private DataTable GetTicketById(string ticket_id)
        {
            DataTable result = new DataTable("Ticket");

            using (SqlConnection c = new SqlConnection(database))
            {
                try
                {
                    c.Open();
                    string sql = "select * from TroubleTickets where Id=@i1";
                    SqlCommand cmd = new SqlCommand(sql, c);
                    cmd.Parameters.AddWithValue("@i1", ticket_id);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(result);
                }
                catch (SqlException sqlEx)
                {
                    Console.WriteLine(sqlEx);
                }
                finally
                {
                    c.Close();
                }
            }
            return result;
        }

        private DataTable GetPersonById(string person_id)
        {
            DataTable result = new DataTable("Person");

            using (SqlConnection c = new SqlConnection(database))
            {
                try
                {
                    c.Open();
                    string sql = "select * from People where Id=@i1";
                    SqlCommand cmd = new SqlCommand(sql, c);
                    cmd.Parameters.AddWithValue("@i1", Int32.Parse(person_id));
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(result);
                }
                catch (SqlException sqlEx)
                {
                    Console.WriteLine(sqlEx);
                }
                finally
                {
                    c.Close();
                }
            }
            return result;
        }


        private string GetDate()
        {
            DateTime date = DateTime.Now;
            string result = date.Day + "-";
            result += date.Month + "-";
            result += date.Year + " ";
            result += date.Hour + ":";
            result += date.Minute + ":";
            result += date.Second;
            return result;
        }
    }
}
