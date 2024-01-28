using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class HackablePlayerIcon : NetworkBehaviour
{
    [SyncVar]
    public int playerID;

    [SyncVar]
    public Color color;

    public Button btn;

    void Start()
    {
        btn = this.GetComponent<Button>();
        btn.onClick.AddListener(clicked);
    }

    void Update()
    {
        if (playerID < 0) {
            btn.enabled = false;
        } else {
            btn.enabled = true;
        }
    }

    public void clicked()
    {
        MessageInput messageInput = GameObject.Find("MessageInput").GetComponent<MessageInput>();
        messageInput.setMessagingAs(playerID, color);
    }
}
