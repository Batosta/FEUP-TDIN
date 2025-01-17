﻿using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

[Serializable]
public class MessageModel
{
    public ObjectId Id { get; set; }

    public string ChatName { get; set; }

    public string Sender { get; set; }

    public string Text { get; set; }
    
    public string Time { get; set; }

    public bool isPrivate { get; set; }

    public List<string> Receivers { get; set; }
}
