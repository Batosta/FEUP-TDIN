using System;
using System.Collections.Generic;
using System.Text;

public class ConversationProposal
{
    IServerObj server;
    readonly string proposalSenderUsername;
    readonly List<string> proposalReceiverUsernames;

    public ConversationProposal(IServerObj server, string proposalSenderUsername, List<string> proposalReceiverUsername)
    {
        this.server = server;
        this.proposalSenderUsername = proposalSenderUsername;
        this.proposalReceiverUsernames = proposalReceiverUsername;
    }

    public void SendConversationProposal()
    {
        server.SendConversationProposal(proposalSenderUsername, proposalReceiverUsernames);
    }
}
