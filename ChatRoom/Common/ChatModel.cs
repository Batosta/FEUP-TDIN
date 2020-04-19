using MongoDB.Bson;
using System;
using System.Collections.Generic;

[Serializable]
public class ChatModel
{
    public ObjectId Id { get; set; }

    public string Name { get; set; }

    public List<string> Users { get; set; }

    public List<MessageModel> Messages { get; set; }
}
