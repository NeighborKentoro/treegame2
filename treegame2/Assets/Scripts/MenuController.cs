using System.Collections.Generic;
using UnityEngine;
using Mirror.Discovery;
using Mirror;

public class MenuController : MonoBehaviour
{
    readonly Dictionary<long, ServerResponse> discoveredServers = new Dictionary<long, ServerResponse>();

    public NetworkDiscovery networkDiscovery;

    public Canvas mainMenuCanvas;

    public Canvas findGameCanvas;

    public Canvas roomCanvas;

    public GameObject hostedGameButton;

    void OnEnable() {
        networkDiscovery.OnServerFound.AddListener(OnDiscoveredServer);
    }

    void OnDisable() {
        networkDiscovery.OnServerFound.RemoveListener(OnDiscoveredServer);
    }

    public void HostGame() {
        discoveredServers.Clear();
        NetworkManager.singleton.StartHost();
        networkDiscovery.AdvertiseServer();
        this.mainMenuCanvas.enabled = false;
        this.roomCanvas.enabled = true;
    }

    public void FindGame() {
        discoveredServers.Clear();
        networkDiscovery.StartDiscovery();
        this.mainMenuCanvas.enabled = false;
        this.findGameCanvas.enabled = true;
    }

    public void LeaveGame() {
        if (NetworkClient.isConnected) {
            if (NetworkServer.active) {
                NetworkManager.singleton.StopHost();
                networkDiscovery.StopDiscovery();

            } else if (NetworkClient.isConnected) {
                NetworkManager.singleton.StopClient();
            }
        }
        this.roomCanvas.enabled = false;
        this.mainMenuCanvas.enabled = true;
    }

    public void Back() {
        networkDiscovery.StopDiscovery();
        this.findGameCanvas.enabled = false;
        this.mainMenuCanvas.enabled = true;
    }

    public void ExitGame() {
        #if UNITY_STANDALONE
            //Quit the application
            Application.Quit();
        #endif
 
        #if UNITY_EDITOR
            //Stop playing the scene
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    public void Connect(ServerResponse info)
    {
        Debug.Log("connecting to host");
        networkDiscovery.StopDiscovery();
        NetworkManager.singleton.StartClient(info.uri);
        this.findGameCanvas.enabled = false;
        this.roomCanvas.enabled = true;
    }

    public void OnDiscoveredServer(ServerResponse info)
    {
        if (discoveredServers.ContainsKey(info.serverId)) {
            discoveredServers[info.serverId] = info;
        } else {
            discoveredServers[info.serverId] = info;
            GameObject gameButton = Instantiate(hostedGameButton, this.findGameCanvas.transform);
            HostedGameButton buttonCmp = gameButton.GetComponent<HostedGameButton>();
            buttonCmp.serverInfo = info;
            buttonCmp.connectToHostEvent += Connect;
            gameButton.SetActive(true);
        }
    }
}
