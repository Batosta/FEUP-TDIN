namespace ChatClient
{
    public interface IChatWindow
    {
        void writeReceivedMessage(string message, string username, bool isPrivate);

        string getID();
    }
}