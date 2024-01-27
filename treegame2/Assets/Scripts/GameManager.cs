using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameManager : NetworkBehaviour
{

    [SyncVar]
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
        //msgObj.transform.position = new Vector2(0, msgObj.transform.position.y);
        


            //GameObject.Instantiate(messageObject);

        // networking
    }
}
