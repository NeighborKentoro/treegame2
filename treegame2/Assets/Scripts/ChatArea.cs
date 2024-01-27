using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatArea : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        RectTransform rect = this.gameObject.GetComponent<RectTransform>();
        Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>(); 
        rect.sizeDelta = new Vector2(1110, player.getRole == Role 1500);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
