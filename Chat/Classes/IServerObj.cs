using System;
using System.Collections.Generic;
using System.Text;

public interface IServerObj
{
    void Register(string username, string realName, string password);

    int Login(string username, string password);

    string HashPassword(string password);
}

