using System;
using System.Collections.Generic;
using System.Text;

public delegate void AlterDelegate(Operation op, string username, string port);

public interface IClientObj
{
    void ReceiveProposal(string proposalSenderAddress);

    void ReceiveYesToProposal(string proposalReceiverUsername, string proposalReceiverAddress);

    void ReceiveNoToProposal(string proposalReceiverUsername);

    void StartAcceptedProposal(string proposalSenderUsername, string proposalSenderAddress);
}
