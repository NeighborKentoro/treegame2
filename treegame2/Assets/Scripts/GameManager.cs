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

    readonly Dictionary<int, int> votesByPlayerID = new Dictionary<int, int>();

    void Start()
    {
        this.currentGameMode = GameMode.MENU;
    }

    void OnEnable() {
        EventManager.changeGameModeEvent += this.ChangeGameMode;
        EventManager.timerExpiredEvent += this.TimerExpired;
        EventManager.playerChatEvent += this.OnSendChat;
        EventManager.voteEvent += this.OnVote;
    }

    void OnDisable() {
        EventManager.changeGameModeEvent -= this.ChangeGameMode;
        EventManager.timerExpiredEvent -= this.TimerExpired;
        EventManager.playerChatEvent -= this.OnSendChat;
        EventManager.voteEvent -= this.OnVote;
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
                    TallyVotes();
                    EventManager.ChangeGameMode(GameMode.CHAT);
                }
                break;
            default:
                break;
        }
    }

    void TallyVotes() {
    
        votesByPlayerID.Clear();
    }

    public void OnVote(int playerID) {
        if (isServer) {
            AddVote(playerID);
        } else {
            RpcVote(playerID);
        }
    }

    void AddVote(int playerID) {
        if (votesByPlayerID.ContainsKey(playerID)) {
            votesByPlayerID[playerID] += 1;
        } else {
            votesByPlayerID.Add(playerID, 1);
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

    [Command]
    public void RpcVote(int playerID)
    {
        AddVote(playerID);
    }
}
