using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public struct CreatePlayerMessage : NetworkMessage { }
public struct PlayerChatMessage : NetworkMessage {
    public int playerID;
    public string message;
    public bool sentByHacker;
}


public class HackerNetworkManager : NetworkManager
{

    public Color[] playerColors = new Color[8];

    public override void OnServerConnect (NetworkConnectionToClient connection) {
        base.OnServerConnect(connection);
        EventManager.OnServerConnect();
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        base.OnServerDisconnect(conn);
        EventManager.OnServerDisconnect();
    }

    public override void OnClientDisconnect() {
        base.OnClientDisconnect();
        EventManager.OnClientDisconnect();
    }

    public override void OnStartServer()
    {
        base.OnStartServer();

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
        GameObject gameobject = Instantiate(playerPrefab);
        Player player = gameobject.GetComponent<Player>();
        player.setColor(this.playerColors[NetworkManager.singleton.numPlayers]);
        player.SetPlayerID(NetworkManager.singleton.numPlayers);
        NetworkServer.AddPlayerForConnection(conn, gameobject);
    }

    void OnPlayerChat(NetworkConnectionToClient conn, PlayerChatMessage message) {
        Debug.Log("Received " + message.message + " from " + message.playerID);
    }
}