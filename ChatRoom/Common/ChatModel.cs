using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

[Serializable]
public class ChatModel
{
    public ObjectId Id { get; set; }
    public string ChatID { get; set; }
    public List<string> Users { get; set; }
    public List<MessageModel> Messages { get; set; }
}
