using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class GameManager : NetworkBehaviour
{

    private GameMode currentGameMode;

    [SerializeField]
    private GameObject messageObject;

    // Start is called before the first frame update
    void Start()
    {
        this.currentGameMode = GameMode.CHAT;
    }

    // Update is called once per frame
    void Update()
    {
    
    }

    void OnGUI()
    {
        if (isServer) {
            if (GUI.Button(new Rect(100, 700, 200, 100), "Change Game Mode")) {
                EventManager.ChangeGameMode(GameMode.CHAT);
                this.RpcChangeGameMode(GameMode.CHAT);
                Debug.Log("NUM PLAYERS: " + NetworkManager.singleton.numPlayers);
            }
        }
    }

    void OnEnable() {
        EventManager.changeGameModeEvent += this.ChangeGameMode;
        EventManager.userSendMessageEvent += this.UserSendMessage;
    }

    void OnDisable() {
        EventManager.changeGameModeEvent -= this.ChangeGameMode;
        EventManager.userSendMessageEvent -= this.UserSendMessage;
    }

    void ChangeGameMode(GameMode gameMode) {
        this.currentGameMode = gameMode;

        if (gameMode == GameMode.CHAT)
        {
            GameObject[] connectedPlayers = GameObject.FindGameObjectsWithTag("ConnectedPlayer");
            GameObject[] hackablePlayers = GameObject.FindGameObjectsWithTag("HackablePlayer");
            for (int i = 0; i < hackablePlayers.Length; i++)
            {
                if (i > connectedPlayers.Length)
                {
                    hackablePlayers[i].SetActive(false);
                }
                else
                {
                    hackablePlayers[i].SetActive(true);
                    hackablePlayers[i].GetComponent<HackablePlayerIcon>().player = connectedPlayers[i];
                    hackablePlayers[i].GetComponent<Image>().color = connectedPlayers[i].GetComponent<Player>().getColor();
                }
            }
        }
    }

    void UserSendMessage(string message)
    {
        // add message to chat thread
        GameObject chatContent = GameObject.FindGameObjectWithTag("ChatContent");
        GameObject msgObj = Instantiate(messageObject, chatContent.transform);
        Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        Message msg = msgObj.GetComponent<Message>();
        msg.setPlayerColor(player.getColor());
        msg.setMessage(message);
        msg.transform.localScale = Vector2.one;

        // networking

    }

    [ClientRpc]
    public void RpcChangeGameMode(GameMode gameMode)
    {
        EventManager.ChangeGameMode(gameMode);
        Debug.Log("Server called me");
    }
}
