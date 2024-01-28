using System.Collections;
using System.Collections.Generic;
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
        for(int i = 0; i < 8; i++) {
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

    void OnPlayerChat(NetworkConnectionToClient conn, PlayerChatMessage message) {
        Debug.Log("Received " + message.message + " from " + message.playerID);
        Color msgColor = this.playerColors[message.playerID];
        bool sentByHacker = this.playerIDByConnectionID[conn.connectionId] != message.playerID;
    }
}