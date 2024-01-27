using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameManager : NetworkBehaviour
{

    [SyncVar]
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

    void OnEnable() {
        EventManager.changeGameModeEvent += this.ChangeGameMode;
    }

    void OnDisable() {
        EventManager.changeGameModeEvent -= this.ChangeGameMode;
    }

    void ChangeGameMode(GameMode gameMode) {
        this.currentGameMode = gameMode;
    }
}
