using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public struct CreatePlayerMessage : NetworkMessage { }


public class HackerNetworkManager : NetworkManager
{

    public Color[] playerColors = new Color[8];

    public override void OnServerConnect (NetworkConnectionToClient connection) {
        base.OnServerConnect(connection);
        Debug.Log("client connected baby");
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        base.OnServerDisconnect(conn);
        Debug.Log("client disconnected baby");
    }

    public override void OnClientDisconnect() {
        base.OnClientDisconnect();
        Debug.Log("server disconnected");
        EventManager.OnClientDisconnect();
    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        NetworkServer.RegisterHandler<CreatePlayerMessage>(OnCreateCharacter);
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
}