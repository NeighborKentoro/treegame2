using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameManager : NetworkBehaviour
{

    private GameMode currentGameMode;

    [SyncVar]
    public int currentRound;

    public int defaultRounds;

    public GameObject messageObject;

    void Start()
    {
        this.currentGameMode = GameMode.MENU;
    }

    void OnEnable() {
        EventManager.changeGameModeEvent += this.ChangeGameMode;
        EventManager.timerExpiredEvent += this.TimerExpired;
        EventManager.playerChatEvent += this.OnSendChat;
    }

    void OnDisable() {
        EventManager.changeGameModeEvent -= this.ChangeGameMode;
        EventManager.timerExpiredEvent -= this.TimerExpired;
        EventManager.playerChatEvent -= this.OnSendChat;
    }

    void ChangeGameMode(GameMode gameMode) {
        this.currentGameMode = gameMode;
        if (isServer) {
            this.RpcChangeGameMode(gameMode);
        }
    }

    void TimerExpired() {
        switch (this.currentGameMode) {
            case GameMode.CHAT:
                EventManager.ChangeGameMode(GameMode.VOTE);
                break;
            case GameMode.VOTE:
                this.currentRound++;
                if (this.currentRound >= this.defaultRounds) {
                    EventManager.ChangeGameMode(GameMode.RESULTS);
                } else {
                    EventManager.ChangeGameMode(GameMode.CHAT);
                }
                break;
            default:
                break;
        }
    }

    public void OnSendChat(string message, Color color, int playerID, bool sentByHacker) {
        RpcSendChatMessage(message, color, playerID, sentByHacker);
    }

    [ClientRpc]
    public void RpcSendChatMessage(string message, Color color, int playerID, bool sentByHacker)
    {
        GameObject chatContent = GameObject.FindGameObjectWithTag("ChatContent");
        GameObject msgObj = Instantiate(messageObject, chatContent.transform);
        Message msg = msgObj.GetComponent<Message>();
        msg.setPlayerColor(color);
        msg.setMessage(message);
        msg.transform.localScale = Vector2.one;
    }

    [ClientRpc]
    public void RpcChangeGameMode(GameMode gameMode)
    {
        if (isClientOnly) {
            EventManager.ChangeGameMode(gameMode);
        }
    }
}
