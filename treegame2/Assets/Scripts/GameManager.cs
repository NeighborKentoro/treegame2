using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameManager : NetworkBehaviour
{

    private GameMode currentGameMode;
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
        if (GUI.Button(new Rect(100, 700, 200, 200), "Change Game Mode")) {
            EventManager.ChangeGameMode(GameMode.CHAT);
            this.RpcChangeGameMode(GameMode.CHAT);
        }
    }

    void OnEnable() {
        EventManager.changeGameModeEvent += this.ChangeGameMode;
    }

    void OnDisable() {
        EventManager.changeGameModeEvent -= this.ChangeGameMode;
    }

    void ChangeGameMode(GameMode gameMode) {
        this.currentGameMode = gameMode;
    }

    [ClientRpc(includeOwner = false)]
    public void RpcChangeGameMode(GameMode gameMode)
    {
        EventManager.ChangeGameMode(gameMode);
        Debug.Log("Server called me");
    }
}
