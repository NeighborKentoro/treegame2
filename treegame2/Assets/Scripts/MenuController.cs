using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    public void HostGame() {
    
    }

    public void FindGame() {
    
    }

    public void ExitGame() {
        #if UNITY_STANDALONE
            //Quit the application
            Application.Quit();
        #endif
 
        #if UNITY_EDITOR
            //Stop playing the scene
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
