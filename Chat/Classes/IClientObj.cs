using System;
using System.Collections.Generic;
using System.Text;

public delegate void MainDelegate(Action action, string username, string port);

public interface IClientObj
{
    //void ReceiveNewSession(string username, string port);
}
