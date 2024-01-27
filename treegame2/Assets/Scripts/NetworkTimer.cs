using UnityEngine;
using TMPro;
using Mirror;
using System;

public class NetworkTimer : NetworkBehaviour
{
    [Range(0,180)]
    public float defaultChatTime = 90f;

    [Range(0,60)]
    public float defaultVoteTime = 20f;

    [SyncVar]
    private float currentTimer;

    private TextMeshProUGUI textMesh;

    // Start is called before the first frame update
    void Start()
    {
        this.textMesh = GetComponentInChildren<TextMeshProUGUI>();   
    }

    // Update is called once per frame
    void Update()
    {
        
        if (this.currentTimer >= 0) {
            this.textMesh.text = Math.Floor(this.currentTimer).ToString();
            this.currentTimer -= Time.deltaTime;
        }
    }

    void OnEnable() {
        EventManager.changeGameModeEvent += this.ChangeGameMode;
    }

    void OnDisable() {
        EventManager.changeGameModeEvent -= this.ChangeGameMode;
    }

    void ChangeGameMode(GameMode gameMode) {
        switch (gameMode) {
            case GameMode.CHAT:
                this.currentTimer = this.defaultChatTime;
                break;
            case GameMode.VOTE:
                this.currentTimer = this.defaultVoteTime;
                break;
            default:
                this.currentTimer = -1;
                this.textMesh.text = "";
                break;
        }
    }
}
