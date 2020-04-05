using MongoDB.Bson;

public class UserModel
{
    public ObjectId Id { get; set; }

    public string Username { get; set; }

    public string RealName { get; set; }

    public string Password { get; set; }
}
