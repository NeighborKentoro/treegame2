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
        member
    }

    [SerializeField,SyncVar]
    private Role role;

    [SerializeField,SyncVar]
    private Color color;

    [SyncVar]
    private int playerID;

    public SpriteRenderer sprite;

    // Start is called before the first frame update
    void Start()
    {
        sprite.color = this.color;
        if (isLocalPlayer)
        {
            GameObject chatArea = GameObject.FindGameObjectWithTag("ChatArea");
            chatArea.GetComponent<RectTransform>().sizeDelta = new Vector2(715, this.role == Role.hacker ? 750 : 1000);

            if (role == Role.member)
            {
                GameObject.Find("HackablePlayers").SetActive(false);
            }
            else
            {
                GameObject.Find("HackablePlayers").SetActive(true);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A)) {
            this.transform.position += new Vector3(0.5f, 0f, 0f);
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

    public void UserSendMessage(string message, int playerID) {
        if (isLocalPlayer) {
            PlayerChatMessage chatMessage = new PlayerChatMessage {
                playerID = playerID,
                message = message
            };
            NetworkClient.Send(chatMessage);
        }
    }
}
