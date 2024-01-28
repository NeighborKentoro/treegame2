using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{

    public delegate void ChangeGameModeAction(GameMode gameMode);
	public static event ChangeGameModeAction changeGameModeEvent;

    public delegate void UserSendMessageAction(string message);
    public static event UserSendMessageAction userSendMessageEvent;

    public delegate void NetworkAction();
    public static event NetworkAction onClientDisconnectEvent;
    public static event NetworkAction onServerConnectEvent;
    public static event NetworkAction onServerDisconnectEvent;

    public delegate void TimerAction();
    public static event TimerAction timerExpiredEvent;

    public static void ChangeGameMode(GameMode gameMode) {
        changeGameModeEvent?.Invoke(gameMode);
    }

    public static void UserSendMessage(string message)
    {
        Debug.Log("user send message " + message);
        userSendMessageEvent?.Invoke(message);
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
}
