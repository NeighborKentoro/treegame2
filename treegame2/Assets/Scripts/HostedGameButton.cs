using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror.Discovery;
using TMPro;

public class HostedGameButton : MonoBehaviour
{

    public delegate void ConnectToHostAction(ServerResponse serverInfo);
	public event ConnectToHostAction connectToHostEvent;

    private ServerResponse serverInfo;

    public TextMeshProUGUI textMesh;

    public void SetServerInfo(ServerResponse info) {
        this.serverInfo = info;
        textMesh.text = info.EndPoint.Address.ToString();
    }

    public void OnClick() {
        connectToHostEvent(serverInfo);
    }

}
