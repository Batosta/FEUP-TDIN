using System;
using System.Collections.Generic;
using System.Text;

public class Repeater : MarshalByRefObject
{
    public event MainDelegate mainDelegate;

    public override object InitializeLifetimeService()
    {
        Console.WriteLine("passa aqui 1");
        return null;
    }

    public void Repeat(Action action, string username, string port)
    {
        Console.WriteLine("passa aqui 2");
        // Checkar esta parte
        mainDelegate?.Invoke(action, username, port);
    }
}
