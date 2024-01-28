using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameManager : NetworkBehaviour
{

    private GameMode currentGameMode;

    [SerializeField]
    private GameObject messageObject;

    [SyncVar]
    public int currentRound;

    public int defaultRounds;

    // Start is called before the first frame update
    void Start()
    {
        this.currentGameMode = GameMode.MENU;
    }

    void OnGUI()
    {
        if (isServer) {
            if (GUI.Button(new Rect(100, 700, 200, 100), "Change Game Mode")) {
                EventManager.ChangeGameMode(GameMode.CHAT);
                Debug.Log("NUM PLAYERS: " + NetworkManager.singleton.numPlayers);
            }
        }
    }

    void OnEnable() {
        EventManager.changeGameModeEvent += this.ChangeGameMode;
        EventManager.userSendMessageEvent += this.UserSendMessage;
        EventManager.timerExpiredEvent += this.TimerExpired;
    }

    void OnDisable() {
        EventManager.changeGameModeEvent -= this.ChangeGameMode;
        EventManager.userSendMessageEvent -= this.UserSendMessage;
        EventManager.timerExpiredEvent -= this.TimerExpired;
    }

    void ChangeGameMode(GameMode gameMode) {
        this.currentGameMode = gameMode;

        switch(gameMode)
        {
          
        }

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

    void UserSendMessage(string message)
    {
        // add message to chat thread
        GameObject chatContent = GameObject.FindGameObjectWithTag("ChatContent");
        GameObject msgObj = Instantiate(messageObject, chatContent.transform);
        Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        Message msg = msgObj.GetComponent<Message>();
        msg.setPlayerColor(player.getColor());
        msg.setMessage(message);
        msg.transform.localScale = Vector2.one;

        // networking

    }

    [ClientRpc]
    public void RpcChangeGameMode(GameMode gameMode)
    {
        if (isClientOnly) {
            EventManager.ChangeGameMode(gameMode);
        }
        Debug.Log("Server called me");
    }
}
