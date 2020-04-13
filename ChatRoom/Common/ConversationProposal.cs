using System;
using System.Collections.Generic;
using System.Text;

public class ConversationProposal
{
    IServerObj server;
    readonly string proposalSenderUsername;
    readonly string proposalReceiverUsername;

    public ConversationProposal(IServerObj server, string proposalSenderUsername, string proposalReceiverUsername)
    {
        this.server = server;
        this.proposalSenderUsername = proposalSenderUsername;
        this.proposalReceiverUsername = proposalReceiverUsername;
    }

    public void SendProposal()
    {
        server.SendProposal(proposalSenderUsername, proposalReceiverUsername);
    }
}
