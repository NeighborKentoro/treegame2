using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;

public class VoteButton : NetworkBehaviour
{
    [SyncVar]
    public int playerID;

    [SyncVar]
    public Color color;

    public Button btn;

    public TextMeshProUGUI textMesh;

    // Start is called before the first frame update
    void Start()
    {
        btn = this.GetComponent<Button>();
        btn.onClick.AddListener(clicked);
        textMesh = this.GetComponentInChildren<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (this.playerID < 0) {
            this.btn.enabled = false;
            this.btn.image.enabled = false;
            this.textMesh.text = "";
        } else {
            this.btn.enabled = true;
            this.textMesh.text = playerID.ToString();
            this.btn.image.color = this.color;
        }
    }

    public void clicked()
    {
        EventManager.OnVote(playerID);
    }
}
