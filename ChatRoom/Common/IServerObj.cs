using System;
using System.Collections.Generic;
using System.Text;

public enum Operation { SessionStart, SessionEnd };

public interface IServerObj
{
    event AlterDelegate alterEvent;



    int Login(string username, string password, string port);

    void PerformLogin(string username, string port);

    List<UserSession> GetActiveSessions();
}
