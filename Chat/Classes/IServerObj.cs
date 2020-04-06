using System;
using System.Collections.Generic;
using System.Text;

public interface IServerObj
{
    int Register(string username, string realName, string password);

    int Login(string username, string password, string host, string port);

    string HashPassword(string password);
    List<UserSession> GetActiveUsersList();
}

