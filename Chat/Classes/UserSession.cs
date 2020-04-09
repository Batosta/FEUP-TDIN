using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

[Serializable]
public struct UserSession : ISerializable
{
    public string username;
    public string port;

    public UserSession(string username, string port)
    {
        this.username = username;
        this.port = port;
    }

    // this constructor is used for deserialization
    public UserSession(SerializationInfo info, StreamingContext text) : this()
    {
        username = info.GetString("username");
        port = info.GetString("port");
    }

    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue("username", username);
        info.AddValue("port", port);
    }
}