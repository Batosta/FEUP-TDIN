using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace TTService {
    public class TTService : ITTService {
        readonly string database;

        TTService() {
            string connection = ConfigurationManager.ConnectionStrings["TTs"].ConnectionString;
            database = String.Format(connection, AppDomain.CurrentDomain.BaseDirectory);
        }

        public int AddTicket(string author, string title, string problem) {
            int id = 0;

            using (SqlConnection c = new SqlConnection(database)) {
                try {
                    c.Open();
                    string sql = "insert into TroubleTickets(Author, Title, Problem, Answer, Status, Date) values (@a1, @t1, @p1, '', 'unassigned', @d1)";     // injection protection
                    SqlCommand cmd = new SqlCommand(sql, c);                                                                                        // injection protection
                    cmd.Parameters.AddWithValue("@a1", author);                                                                                     // injection protection
                    cmd.Parameters.AddWithValue("@t1", title);                                                                                      // injection protection
                    cmd.Parameters.AddWithValue("@p1", problem);                                                                                    // injection protection
                    cmd.Parameters.AddWithValue("@d1", GetDate());                                                                                    // injection protection
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "select max(Id) from TroubleTickets";
                    id = (int)cmd.ExecuteScalar();
                }
                catch (SqlException sqlEx)
                {
                    Console.WriteLine(sqlEx);
                }
                finally {
                    c.Close();
                }
            }
            return id;
        }

        public DataTable GetTickets(string author) {
            DataTable result = new DataTable("TroubleTickets");

            using (SqlConnection c = new SqlConnection(database)) {
                try {
                    c.Open();
                    string sql = "select Id, Title, Problem, Status, Answer, Date from TroubleTickets where Author=@a1";
                    SqlCommand cmd = new SqlCommand(sql, c);
                    cmd.Parameters.AddWithValue("@a1", author);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(result);
                }
                catch (SqlException sqlEx) {
                    Console.WriteLine(sqlEx);
                }
                finally {
                    c.Close();
                }
            }
            return result;
        }

        public DataTable GetUsers() {
            DataTable result = new DataTable("Authors");

            using (SqlConnection c = new SqlConnection(database)) {
                try {
                    c.Open();
                    string sql = "select * from Authors";
                    SqlCommand cmd = new SqlCommand(sql, c);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(result);
                }
                catch (SqlException sqlEx)
                {
                    Console.WriteLine(sqlEx);
                }
                finally {
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
