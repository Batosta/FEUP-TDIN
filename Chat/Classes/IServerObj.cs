using System;
using System.Collections.Generic;
using System.Text;

public interface IServerObj
{
    void Register(string username, string realName, string password);

    string HashPassword(string password);
}

