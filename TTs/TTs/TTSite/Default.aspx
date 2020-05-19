<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>TTSystem</title>
    <style type="text/css">
      .auto-style1 {
        width: 100%;
      }
      .auto-style3 {
        width: 65px;
        font-family: Arial;
      }
    </style>
</head>
<body style="background-color: #CCFFCC">
  <form id="form1" runat="server">
    <div>
    
      <h1>Acme Corporation TTSystem</h1>
      <table class="auto-style1">
        <tr>
          <td class="auto-style3">Author:</td>
          <td>
            <asp:DropDownList ID="DropDownList1" runat="server" DataTextField="Name" DataValueField="Id">
            </asp:DropDownList>
          </td>
        </tr>
        <tr>
          <td class="auto-style3">Problem: </td>
          <td>
            <asp:TextBox ID="TextBox1" runat="server" Height="100px" TextMode="MultiLine" Width="600px"></asp:TextBox>
              &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button ID="Button1" runat="server" Text="Submit" OnClick="Button1_Click" />
          </td>
        </tr>
      </table>
      <p>
        <asp:Label ID="Label1" runat="server" Text="Result:" Font-Bold="True" Font-Names="Arial" ForeColor="#0000CC"></asp:Label>
      </p>
      <p>
        <asp:Button ID="Button2" runat="server" OnClick="Button2_Click" Text="Submitted problems" />
      </p>
      <p>
        <asp:GridView ID="GridView1" runat="server"></asp:GridView>
      </p>
      <p>
        <asp:Label ID="Label2" runat="server" Font-Bold="True" Font-Names="Arial" ForeColor="Red"></asp:Label>
      </p>

    </div>
  </form>
</body>
</html>