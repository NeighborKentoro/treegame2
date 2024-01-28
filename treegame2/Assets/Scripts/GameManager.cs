using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using System;
using TMPro;

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

    void ChangeGameMode(GameMode gameMode, Player.Role winner) {
        this.currentGameMode = gameMode;

        switch (winner)
        {
            case Player.Role.hacker:
                GameObject.Find("WinnerText").GetComponent<TMP_Text>().text = "Hackers win!";
                break;
            case Player.Role.member:
                GameObject.Find("WinnerText").GetComponent<TMP_Text>().text = "Hackers have been eliminated! Group chat wins!";
                break;
        }

        if (isServer) {
            this.RpcChangeGameMode(gameMode, winner);
        }
    }

    void TimerExpired() {
        switch (this.currentGameMode) {
            case GameMode.CHAT:
                EventManager.ChangeGameMode(GameMode.VOTE, Player.Role.none);
                break;
            case GameMode.VOTE:
                this.currentRound++;
                TallyVotes();
                evaluateWinCondition();
                break;
            default:
                break;
        }
    }

    void TallyVotes() {
    
        // total number of votes made
        int totalNumberOfVotes = 0;

        // all players in game
        GameObject[] connectedPlayers = GameObject.FindGameObjectsWithTag("ConnectedPlayer");

        // get player objects in a sorted array
        Player[] players = GameObject.Find("NetworkManager").GetComponent<HackerNetworkManager>().orderPlayerArray(connectedPlayers);

        // get number of unkicked players who can vote
        int totalActivePlayers = 0;
        foreach (Player player in players) { 
            if (player.GetKicked() == false)
            {
                totalActivePlayers++;
            }
        }

        // get total number of votes
        if (votesByPlayerID.Count > 0)
        {
            int playerWithMostVotes = -1;
            int prevVotes = 0;
            foreach (KeyValuePair<int, int> entry in votesByPlayerID)
            {
                totalNumberOfVotes += entry.Value;
                if (entry.Value > prevVotes)
                {
                    prevVotes = entry.Value;
                    playerWithMostVotes = entry.Key;
                }
            }

            // number of votes must be greater than the number of active players
            if (Math.Floor((Decimal)(totalNumberOfVotes / totalActivePlayers)) * 100 > 50)
            {
                // set player with most votes to be kicked
                KickPlayer(playerWithMostVotes);
                Debug.Log("Kick player " + playerWithMostVotes);
            }
        }

        votesByPlayerID.Clear();
    }

    public void evaluateWinCondition()
    {
        // all players in game
        GameObject[] connectedPlayers = GameObject.FindGameObjectsWithTag("ConnectedPlayer");

        // get player objects in a sorted array
        Player[] players = GameObject.Find("NetworkManager").GetComponent<HackerNetworkManager>().orderPlayerArray(connectedPlayers);



        // get unkicked players 
        // ArrayList unKickedPlayers = new ArrayList();
        int hackersLeft = 0;
        int playersLeft = 0;
        foreach (Player player in players)
        {
            if (player.GetKicked() == false)
            {
                if (player.getRole() == Player.Role.hacker)
                {
                    hackersLeft++;
                } else
                {
                    playersLeft++;
                }
            }
        }


        // Player win condition - no hackers left
        bool gameOver = false;
        if (hackersLeft == 0 && !gameOver)
        {
            // PLAYER WINS BABBBY
            EventManager.ChangeGameMode(GameMode.RESULTS, Player.Role.member);
            gameOver = true;
        }

        if ((playersLeft == 0 || currentRound >= defaultRounds && hackersLeft > 0) && !gameOver)
        {
            // HACKERS WIN BABBBBY
            EventManager.ChangeGameMode(GameMode.RESULTS, Player.Role.hacker);
            gameOver=true;
        }

        if (currentRound < defaultRounds && !gameOver)
        {
            EventManager.ChangeGameMode(GameMode.CHAT, Player.Role.none);
        }
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
    public void RpcChangeGameMode(GameMode gameMode, Player.Role winner)
    {
        if (isClientOnly) {
            EventManager.ChangeGameMode(gameMode,winner);
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
                player.KickMe();
                RpcSendChatMessage("Player " + playerID  + " was removed from the chat", Color.white, -1, false);
            }
        }
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
