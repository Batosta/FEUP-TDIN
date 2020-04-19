public interface IChatWindow
{
    void WriteReceivedMessage(string username, string messageText, string messageTime);

    string GetChatName();

    void LeaveConversation();
}
