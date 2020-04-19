namespace ChatClient
{
    public interface IChatWindow
    {
        void writeReceivedMessage(string message,string time, string username, bool isPrivate);
        string getID();
        void userByebyed();
        void receiveFile(byte[] file, string extension);
    }
}