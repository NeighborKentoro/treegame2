using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;

public struct CreatePlayerMessage : NetworkMessage { }
public struct PlayerChatMessage : NetworkMessage {
    public int playerID;
    public string message;
}


public class HackerNetworkManager : NetworkManager
{
    readonly Stack<int> availablePlayerIDs = new Stack<int>();
    readonly Dictionary<int, int> playerIDByConnectionID = new Dictionary<int, int>();
    public Color[] playerColors = new Color[8];

    void OnEnable() {
        EventManager.onStartGameEvent += OnStartGame;
    }
    void OnDisable() {
        EventManager.onStartGameEvent -= OnStartGame;
    }


    public override void OnServerConnect (NetworkConnectionToClient connection) {
        base.OnServerConnect(connection);
        playerIDByConnectionID.Add(connection.connectionId, availablePlayerIDs.Pop());
        EventManager.OnServerConnect();
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        base.OnServerDisconnect(conn);
        availablePlayerIDs.Push(playerIDByConnectionID[conn.connectionId]);
        playerIDByConnectionID.Remove(conn.connectionId);
        EventManager.OnServerDisconnect();
    }

    public override void OnClientDisconnect() {
        base.OnClientDisconnect();
        EventManager.OnClientDisconnect();
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        for(int i = 7; i >= 0; i--) {
            availablePlayerIDs.Push(i);
        }
        NetworkServer.RegisterHandler<CreatePlayerMessage>(OnCreateCharacter);
        NetworkServer.RegisterHandler<PlayerChatMessage>(OnPlayerChat);
    }

    public override void OnClientConnect()
    {
        base.OnClientConnect();

        CreatePlayerMessage characterMessage = new CreatePlayerMessage { };

        NetworkClient.Send(characterMessage);
    }

    void OnCreateCharacter(NetworkConnectionToClient conn, CreatePlayerMessage message)
    {
        int playerID = playerIDByConnectionID[conn.connectionId];
        GameObject gameobject = Instantiate(playerPrefab);
        Player player = gameobject.GetComponent<Player>();
        player.setColor(this.playerColors[playerID]);
        player.SetPlayerID(playerID);
        NetworkServer.AddPlayerForConnection(conn, gameobject);
    }

    void OnPlayerChat(NetworkConnectionToClient conn, PlayerChatMessage pcm) {
        Color msgColor = this.playerColors[pcm.playerID];
        bool sentByHacker = this.playerIDByConnectionID[conn.connectionId] != pcm.playerID;
        EventManager.OnPlayerChat(pcm.message, msgColor, pcm.playerID, sentByHacker);
    }

    public void OnStartGame() {
        GameObject[] players = GameObject.FindGameObjectsWithTag("ConnectedPlayer");
        int hackerCount = playerIDByConnectionID.Count == 8 ? 3 : 2;
        HashSet<int> hackerIDs = new HashSet<int>();
        while (hackerIDs.Count < hackerCount) {
            int randomID = Random.Range(0, playerIDByConnectionID.Count);
            if (!hackerIDs.Contains(randomID)) {
                hackerIDs.Add(randomID);
                players[randomID].GetComponent<Player>().SetRole(Player.Role.hacker);
            }
        }
        
        GameObject[] hackButtons = GameObject.FindGameObjectsWithTag("HackablePlayer");
        // GameObject[] voteButtons = GameObject.FindGameObjectsWithTag("VotePlayer");
        for(int i = 0; i < 8; i++) {
            GameObject hackButton = hackButtons[i];
            // GameObject voteButton = voteButtons[i];
            HackablePlayerIcon hpi = hackButton.GetComponent<HackablePlayerIcon>();
            if (i < playerIDByConnectionID.Count) {
                hpi.btn.enabled = true;
                hpi.btn.image.enabled = true;
                int playerID = playerIDByConnectionID.Values.ElementAt(i);
                hpi.playerID = playerID;
                hpi.textMesh.text = playerID.ToString();
                hpi.btn.image.color = this.playerColors[playerID];
            } else {
                hpi.playerID = -1;
                hpi.btn.enabled = false;
                hpi.btn.image.enabled = false;
                hpi.textMesh.text = "";
            }
        }
    }
}