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

    void Update() {
        if (this.playerID < 0) {
            this.btn.enabled = false;
            this.btn.image.enabled = false;
            this.textMesh.text = "";
        } else {
            // this.enabled = true;
            this.btn.enabled = true;
            this.textMesh.text = playerID.ToString();
            this.btn.image.color = this.color;
        }
    }

    public void clicked()
    {
        MessageInput messageInput = GameObject.Find("MessageInput").GetComponent<MessageInput>();
        messageInput.setMessagingAs(playerID, color);
    }
}
