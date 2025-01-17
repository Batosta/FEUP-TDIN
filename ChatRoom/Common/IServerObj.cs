﻿using System;
using System.Collections.Generic;
using System.Text;

public enum Operation { SessionStart, SessionEnd };

public interface IServerObj
{
    event AlterDelegate alterEvent;

    /*
     * Login + Logout + Register methods
     */
    int Login(string username, string password, string port);
    
    void PerformLogin(string username, string port);
    
    int Register(string username, string realName, string password);

    void Logout(string username);


    /*
     * Conversation Initiation methods
     */
    void SendConversationProposal(string proposalSenderUsername, List<string> proposalReceiverUsernames);

    void YesToProposal(string proposalSenderUsername, string proposalReceiverUsername);

    void NoToProposal(string proposalSenderUsername, string proposalReceiverUsername);


    /*
     * Other methods
     */
    void StoreMessage(string chatName, string username, string messageText, string messageTime, bool isPrivate, List<string> otherUsernames);

    List<UserSession> GetActiveSessions();
}
