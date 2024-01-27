using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager {

    public delegate void ChangeGameModeAction(GameMode gameMode);
	public static event ChangeGameModeAction changeGameModeEvent;

    public static void ChangeGameMode(GameMode gameMode) {
        changeGameModeEvent?.Invoke(gameMode);
    }

}
