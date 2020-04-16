using System;
using System.Collections.Generic;
using System.Text;

public delegate void AlterDelegate(Operation op, string username, string port);

public interface IClientObj
{

    void receiveMessage(string chatID, string message, string username, bool isPrivate);
    string test(string test);
    void ReceiveGroupProposal(string proposalSenderAddress, List<string> porposalReceiverUsernames);
    void ReceiveProposal(string proposalSenderAddress);

    void ReceiveYesToProposal(string proposalReceiverUsername, string proposalReceiverAddress);

    void ReceiveNoToProposal(string proposalReceiverUsername);

    void StartAcceptedProposal(string proposalSenderUsername, string proposalSenderAddress);

    void StartGroupChat(List<string> proposalSenderUsername, List<string> proposalReceiversAddresses);
}
