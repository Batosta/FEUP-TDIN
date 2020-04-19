using System.Collections.Generic;

public interface IChatWindow
{
    void WriteReceivedMessage(string username, string messageText, string messageTime);

    string GetChatName();

    List<string> GetOtherUsernames();

    void LeaveConversation();
}
