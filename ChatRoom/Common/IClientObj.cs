using System;
using System.Collections.Generic;
using System.Text;

public delegate void AlterDelegate(Operation op, string username, string port);

public interface IClientObj
{

    void receiveMessage(string chatID, string message,string time, string username, bool isPrivate);
    string test(string test);
    void ReceiveGroupProposal(string proposalSenderAddress, List<string> porposalReceiverUsernames);
    void ReceiveProposal(string proposalSenderAddress);

    void ReceiveYesToProposal(string proposalReceiverUsername, string proposalReceiverAddress, string chatID, ChatModel chatmodel);

    void ReceiveNoToProposal(string proposalReceiverUsername);

    void StartAcceptedProposal(string proposalSenderUsername, string proposalSenderAddress, string chatID, ChatModel chatmodel);

    void StartGroupChat(List<string> proposalSenderUsername, List<string> proposalReceiversAddresses, string chatID, ChatModel chatmodel);
    void ByeBye(string chatID);
    void receiveFile(string chatID, byte[] fileToSend, string v1, string username, string username1, bool v2);
}
