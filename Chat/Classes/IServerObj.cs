using System;
using System.Collections.Generic;
using System.Text;

public enum Action { SessionStart, SessionEnd };

public interface IServerObj
{
    event MainDelegate mainDelegate;



    int Register(string username, string realName, string password);

    int Login(string username, string password, string port);

    void PerformLogin(string username, string port);

    string HashPassword(string password);

    List<UserSession> GetActiveUsersList();
}

