using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{

    public delegate void ChangeGameModeAction(GameMode gameMode);
	public static event ChangeGameModeAction changeGameModeEvent;

    public delegate void UserSendMessageAction(string message);
    public static event UserSendMessageAction userSendMessageEvent;

    public static void ChangeGameMode(GameMode gameMode) {
        changeGameModeEvent?.Invoke(gameMode);
    }

    public static void UserSendMessage(string message)
    {
        Debug.Log("user send message " + message);
        userSendMessageEvent?.Invoke(message);
    }

}
