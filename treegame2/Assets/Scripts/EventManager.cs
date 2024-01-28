using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{

    public delegate void ChangeGameModeAction(GameMode gameMode);
	public static event ChangeGameModeAction changeGameModeEvent;

    public delegate void UserSendMessageAction(string message, int playerID);
    public static event UserSendMessageAction userSendMessageEvent;

    public delegate void PlayerChatAction(string message, Color color, int playerID, bool sentByHacker);
    public static event PlayerChatAction playerChatEvent;

    public delegate void VoteAction(int playerID);
    public static event VoteAction voteEvent;

    public delegate void NetworkAction();
    public static event NetworkAction onClientDisconnectEvent;
    public static event NetworkAction onServerConnectEvent;
    public static event NetworkAction onServerDisconnectEvent;
    public static event NetworkAction onStartGameEvent;

    public delegate void TimerAction();
    public static event TimerAction timerExpiredEvent;

    public static void ChangeGameMode(GameMode gameMode) {
        changeGameModeEvent?.Invoke(gameMode);
    }

    public static void UserSendMessage(string message, int playerID)
    {
        userSendMessageEvent?.Invoke(message, playerID);
    }

    public static void OnClientDisconnect() {
        onClientDisconnectEvent?.Invoke();
    }
    public static void OnServerConnect() {
        onServerConnectEvent?.Invoke();
    }
    public static void OnServerDisconnect() {
        onServerDisconnectEvent?.Invoke();
    }

    public static void OnTimerExpired() {
        timerExpiredEvent?.Invoke();
    }

    public static void OnStartGame() {
        onStartGameEvent?.Invoke();
    }

    public static void OnPlayerChat(string message, Color color, int playerID, bool sentByHacker) {
        playerChatEvent?.Invoke(message, color, playerID, sentByHacker);
    }

    public static void OnVote(int playerID) {
        voteEvent?.Invoke(playerID);
    }
}
