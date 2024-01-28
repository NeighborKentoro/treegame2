using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Player : NetworkBehaviour
{
    [SerializeField]
    public enum Role
    {
        hacker,
        member,
        none
    }

    [SerializeField,SyncVar]
    private Role role = Role.member;

    [SerializeField,SyncVar]
    private Color color;

    [SyncVar]
    private int playerID;

    [SyncVar]
    private bool kicked;

    public SpriteRenderer sprite;

    GameObject hackablePlayers;

    RectTransform rectTrans;

    void Start()
    {
        sprite.color = this.color;
        hackablePlayers = GameObject.Find("HackablePlayers");
        GameObject chatArea = GameObject.FindGameObjectWithTag("ChatArea");
        rectTrans = chatArea.GetComponent<RectTransform>();
    }

    void Update() {
        if (isLocalPlayer) {
            hackablePlayers.SetActive(role == Role.hacker);
            rectTrans.sizeDelta = new Vector2(715, this.role == Role.hacker ? 750 : 1000);
        }
    }

    void OnEnable() {
        EventManager.userSendMessageEvent += this.UserSendMessage;
    }

    void OnDisable() {
        EventManager.userSendMessageEvent -= this.UserSendMessage;
    }

    public Color getColor()
    {
        return this.color;
    }

    public void setColor(Color color)
    {
        this.color = color;
    }

    public int GetPlayerID() {
        return this.playerID;
    }

    public void SetPlayerID(int id) {
        this.playerID = id;
    }

    public Role getRole()
    {
        return this.role;
    }

    public void SetRole(Role role) {
        this.role = role;
    }

    public bool GetKicked() {
        return this.kicked;
    }

    public void SetKicked(bool kicked) {
        this.kicked = kicked;
    }

    public void UserSendMessage(string message, int playerID) {
        if (isLocalPlayer) {
            PlayerChatMessage chatMessage = new PlayerChatMessage {
                playerID = playerID,
                message = message
            };
            NetworkClient.Send(chatMessage);
        }
    }

    public void FixPlayerUI () {
        if (isLocalPlayer) {
            MessageInput messageInput = GameObject.Find("MessageInput").GetComponent<MessageInput>();
            messageInput.setMessagingAs(playerID, color);
            
        }
    }
}
