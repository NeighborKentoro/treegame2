using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror.Discovery;
using Mirror;

public class HostedGameButton : MonoBehaviour
{

    public delegate void ConnectToHostAction(ServerResponse serverInfo);
	public event ConnectToHostAction connectToHostEvent;

    public ServerResponse serverInfo;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClick() {
        connectToHostEvent(serverInfo);
    }

}
