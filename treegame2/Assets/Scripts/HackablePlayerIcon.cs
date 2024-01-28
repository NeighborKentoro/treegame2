using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HackablePlayerIcon : NetworkBehaviour
{
    [SyncVar]
    public int playerID;

    [SyncVar]
    public Color color;

    public Button btn;

    public TextMeshProUGUI textMesh;

    void Start()
    {
        btn = this.GetComponent<Button>();
        btn.onClick.AddListener(clicked);
        textMesh = this.GetComponentInChildren<TextMeshProUGUI>();
    }

    public void clicked()
    {
        MessageInput messageInput = GameObject.Find("MessageInput").GetComponent<MessageInput>();
        messageInput.setMessagingAs(playerID, color);
    }
}
