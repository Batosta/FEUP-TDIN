using MongoDB.Bson;
using System.Collections.Generic;

public class UserModel
{
    public ObjectId Id { get; set; }

    public string Username { get; set; }

    public string RealName { get; set; }

    public string Password { get; set; }
    public List<string> ChatIDs { get; set; }
}
