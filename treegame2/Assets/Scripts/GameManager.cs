using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class GameManager : NetworkBehaviour
{

    private GameMode currentGameMode;

    [SyncVar]
    public int currentRound;

    public int defaultRounds;

    public GameObject messageObject;

    readonly Dictionary<int, int> votesByPlayerID = new Dictionary<int, int>();

    public static GameManager singleton { get; private set; }

    private void Awake() 
    { 
        if (singleton != null && singleton != this) { 
            Destroy(this); 
        } else { 
            singleton = this; 
        } 
    }

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
        Player localPlayer = FindLocalPlayer();
        if (playerID > -1 && localPlayer.GetPlayerID() == playerID && sentByHacker) {
            return;
        }

        GameObject chatContent = GameObject.FindGameObjectWithTag("ChatContent");
        GameObject msgObj = Instantiate(messageObject, chatContent.transform);
        Message msg = msgObj.GetComponent<Message>();
        msg.SetPlayerID(playerID);
        msg.setPlayerColor(color);
        msg.setMessage(message);
        msg.SetSentByHacker(sentByHacker);
        msg.transform.localScale = Vector2.one;

        ScrollRect chatArea = GameObject.FindGameObjectWithTag("ChatArea").GetComponent<ScrollRect>();
        chatArea.verticalNormalizedPosition = -0.35f;
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

    [ClientRpc]
    public void RpcFixPlayerUI()
    {
        GameObject[] playerGameObjects = GameObject.FindGameObjectsWithTag("ConnectedPlayer");
        foreach (GameObject pgo in playerGameObjects) {
            pgo.GetComponent<Player>().FixPlayerUI();
        }
    }

    public void KickPlayer(int playerID) {
        GameObject[] playerGameObjects = GameObject.FindGameObjectsWithTag("ConnectedPlayer");
        foreach (GameObject pgo in playerGameObjects) {
            Player player = pgo.GetComponent<Player>();
            if (playerID == player.GetPlayerID()) {
                player.SetKicked(true);
                RpcSendChatMessage("Player " + playerID  + " was removed from the chat", Color.white, -1, false);
            }
        }
    }

    void OnGUI () {
        if (GUI.Button(new Rect(0, 0, 100, 100), "Kick Player"))
            KickPlayer(1);
    }

    public Player FindLocalPlayer() {
        GameObject[] playerGameObjects = GameObject.FindGameObjectsWithTag("ConnectedPlayer");
        foreach (GameObject pgo in playerGameObjects) {
            Player player = pgo.GetComponent<Player>();
            if (player.isLocalPlayer) {
                return player;
            }
        }
        return null;
    }
}
