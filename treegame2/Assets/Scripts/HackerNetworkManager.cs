using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class HackerNetworkManager : NetworkManager
{
    public override void OnStartClient() {

    }

    public override void OnServerConnect (NetworkConnectionToClient connection) {
        Debug.Log("client connected baby");
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        NetworkServer.DestroyPlayerForConnection(conn);
        Debug.Log("client disconnected baby");
    }


}
