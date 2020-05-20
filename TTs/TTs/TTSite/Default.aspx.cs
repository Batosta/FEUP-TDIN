using System;
using System.Drawing;
using System.Web.UI;
using System.ServiceModel;
using TTService;
using System.Data;

public partial class _Default : Page {
    TTProxy proxy;

    protected void Page_Load(object sender, EventArgs e) 
    {
        proxy = new TTProxy();
        if (!Page.IsPostBack) 
        {                           // only on first request of a session
            DropDownList1.DataSource = proxy.GetPeopleByRole("worker");
            DropDownList1.DataBind();
        }
    }

    protected void Button1_Click(object sender, EventArgs e) {
        int id;

        if (TextBox1.Text.Length > 0) 
        {
            if(TextBox2.Text.Length > 0)
            {
                id = proxy.AddTicket(DropDownList1.SelectedValue, TextBox1.Text, TextBox2.Text);
                Label1.ForeColor = Color.DarkBlue;
                Label1.Text = "Result: Inserted with Id = " + id;
                TextBox1.Text = "";
                TextBox2.Text = "";
            }
            else
            {
                Label1.ForeColor = Color.Red;
                Label1.Text = "Result: Please describe a problem!";
            }
        }
        else 
        {
            Label1.ForeColor = Color.Red;
            Label1.Text = "Result: Please describe a title!";
        }
    }

    protected void Button2_Click(object sender, EventArgs e) {
        GridView1.DataSource = proxy.GetTicketsByAuthor(DropDownList1.SelectedValue);
        GridView1.DataBind();
        GridView1.Visible = true;
        Label2.Text = "";
    }
}

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

    public DataTable GetUnassignedTickets()
    {
        return Channel.GetUnassignedTickets();
    }

    public DataTable GetTicketsByAuthor(string author)
    {
        return Channel.GetTicketsByAuthor(author);
    }

    public DataTable GetTicketsBySolver(string role)
    {
        return Channel.GetTicketsBySolver(role);
    }

    public DataTable GetPeopleByRole(string role)
    {
        return Channel.GetPeopleByRole(role);
    }
}

