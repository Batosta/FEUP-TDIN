using System;
using System.Collections.Generic;
using System.Text;

public class AlterEventRepeater : MarshalByRefObject
{
    public event AlterDelegate alterEvent;

    public override object InitializeLifetimeService()
    {
        return null;
    }

    public void Repeater(Operation op, string username, string port)
    {
        if(alterEvent != null)
        {
            alterEvent(op, username, port);
        }
    }
}
