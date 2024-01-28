using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HackablePlayerIcon : MonoBehaviour
{
    [SerializeField]
    public GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        //if (GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().getRole() == Player.Role.hacker)
        //{
        //    this.gameObject.SetActive(true);
        //} else
        //{
        //    this.gameObject.SetActive(false);
        //}
        Button btn = this.GetComponent<Button>();
        btn.onClick.AddListener(clicked);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void clicked()
    {
        MessageInput messageInput = GameObject.Find("MessageInput").GetComponent<MessageInput>();
        messageInput.setMessagingAs(player.GetComponent<Player>());
    }
}
