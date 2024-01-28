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
        
    }

    public void clicked()
    {
        EventManager.OnVote(playerID);
    }
}
