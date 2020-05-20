using System.Data;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace TTService
{
    [ServiceContract]
    public interface ITTService
    {
        [WebInvoke(Method = "POST", UriTemplate = "/tickets", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        int AddTicket(string author, string title, string problem);

        [WebInvoke(Method = "PUT", UriTemplate = "/tickets/{ticket_id}/{solver_name}", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        void AssignTicketToSolver(string solver_name, string ticket_id);

        [WebInvoke(Method = "PUT", UriTemplate = "/tickets_answer/{ticket_id}/{answer}", BodyStyle = WebMessageBodyStyle.WrappedRequest, RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        void AnswerToTicket(string answer, string ticket_id);

        [WebGet(UriTemplate = "/tickets", ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        DataTable GetUnassignedTickets();

        [WebGet(UriTemplate = "/tickets/{author}", ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        DataTable GetTicketsByAuthor(string author);

        [WebGet(UriTemplate = "/assigned_tickets/{solver}", ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        DataTable GetTicketsBySolver(string solver);

        [WebGet(UriTemplate = "/users/{role}", ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        DataTable GetPeopleByRole(string role);
    }
}
