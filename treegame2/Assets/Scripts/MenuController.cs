using System.Collections.Generic;
using UnityEngine;
using Mirror.Discovery;
using Mirror;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    readonly Dictionary<long, ServerResponse> discoveredServers = new Dictionary<long, ServerResponse>();

    public NetworkDiscovery networkDiscovery;

    public Canvas mainMenuCanvas;

    public Canvas findGameCanvas;

    public Canvas roomCanvas;

    public Canvas chatCanvas;

    public GameObject hostedGameButton;

    public Button startGameButton;

    void OnEnable() {
        networkDiscovery.OnServerFound.AddListener(OnDiscoveredServer);
        EventManager.onClientDisconnectEvent += OnClientDisconnect;
        EventManager.onServerConnectEvent += OnServerConnect;
        EventManager.onServerDisconnectEvent += OnServerDisconnect;
    }

    void OnDisable() {
        networkDiscovery.OnServerFound.RemoveListener(OnDiscoveredServer);
        EventManager.onClientDisconnectEvent -= OnClientDisconnect;
        EventManager.onServerConnectEvent -= OnServerConnect;
        EventManager.onServerDisconnectEvent -= OnServerDisconnect;
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

    public void StartGame() {
        EventManager.ChangeGameMode(GameMode.CHAT);
        this.roomCanvas.enabled = false;
        this.chatCanvas.enabled = true;
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

    public void OnClientDisconnect() {
        this.roomCanvas.enabled = false;
        this.mainMenuCanvas.enabled = true;
    }

    public void Connect(ServerResponse info)
    {
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
            buttonCmp.SetServerInfo(info);
            buttonCmp.connectToHostEvent += Connect;
            gameButton.SetActive(true);
        }
    }

    public void OnServerConnect() {
        //Server doesn't call this - so for 5 player threshold subtract 1
        if (NetworkManager.singleton.numPlayers >= 1) {
            this.startGameButton.interactable = true;
            Debug.Log(NetworkManager.singleton.numPlayers);
        }
    }
    public void OnServerDisconnect() {
        if (NetworkManager.singleton.numPlayers < 1) {
            this.startGameButton.interactable = false;
        }
    }
}
