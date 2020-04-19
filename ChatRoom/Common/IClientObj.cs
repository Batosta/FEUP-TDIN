using System;
using System.Collections.Generic;
using System.Text;

public delegate void AlterDelegate(Operation op, string username, string port);

public interface IClientObj
{
    void ReceiveProposal(string proposalSenderAddress, List<string> proposalReceiverUsernames);

    void StartChat(string chatName, List<string> usernames, List<string> addresses, List<MessageModel> previousMessages);

    void ReceiveMessage(string chatName, string username, string messageText, string messageTime, bool isPrivate);

    void LeaveConversation(string chatName);
    void ReceiveFile(string chatName, string username, byte[] fileToSend, string extension, string messageTime);
    void SetupConnection();
}
