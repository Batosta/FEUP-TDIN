using System;
using System.Drawing;
using System.Web.UI;
using System.ServiceModel;
using TTService;
using System.Data;

public partial class _Default : Page {
  TTProxy proxy;

  protected void Page_Load(object sender, EventArgs e) {
    proxy = new TTProxy();
    if (!Page.IsPostBack) {                           // only on first request of a session
      DropDownList1.DataSource = proxy.GetUsers();
      DropDownList1.DataBind();
    }
  }

  protected void Button1_Click(object sender, EventArgs e) {
    int id;

    if (DropDownList1.SelectedIndex > 0) {
      if (TextBox1.Text.Length > 0) {
        id = proxy.AddTicket(DropDownList1.SelectedValue, TextBox1.Text);
        Label1.ForeColor = Color.DarkBlue;
        Label1.Text = "Result: Inserted with Id = " + id;
      }
      else {
        Label1.ForeColor = Color.Red;
        Label1.Text = "Result: Please describe a problem!";
      }
    }
    else {
      Label1.ForeColor = Color.Red;
      Label1.Text = "Result: Select an Author!";
    }
  }

  protected void Button2_Click(object sender, EventArgs e) {
    if (DropDownList1.SelectedIndex > 0) {
      GridView1.DataSource = proxy.GetTickets(DropDownList1.SelectedValue);
      GridView1.DataBind();
      GridView1.Visible = true;
      Label2.Text = "";
    }
    else {
      GridView1.Visible = false;
      Label2.Text = "Select an Author!";
    }
  }
}

class TTProxy : ClientBase<ITTService>, ITTService {
  public DataTable GetUsers() {
    return Channel.GetUsers();
  }

  public DataTable GetTickets(string author) {
    return Channel.GetTickets(author);
  }

  public int AddTicket(string author, string desc) {
    return Channel.AddTicket(author, desc);
  }
}

