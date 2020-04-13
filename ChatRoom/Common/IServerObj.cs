using System;
using System.Collections.Generic;
using System.Text;

public enum Operation { SessionStart, SessionEnd };

public interface IServerObj
{
    event AlterDelegate alterEvent;

    /*
     * Login + Register methods
     */
    int Login(string username, string password, string port);
    void PerformLogin(string username, string port);
    int Register(string username, string realName, string password);


    /*
     * Conversation Initiation methods
     */
    void SendProposal(string proposalSenderUsername, string proposalReceiverUsername);
    void YesToProposal(string proposalSenderUsername, string proposalReceiverUsername);
    void NoToProposal(string proposalSenderUsername, string proposalReceiverUsername);



    List<UserSession> GetActiveSessions();
}
