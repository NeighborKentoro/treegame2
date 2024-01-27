using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Message : MonoBehaviour
{
    private Color playerColor;
    private string message;
    private bool sentByHacker;

    [SerializeField]
    private GameObject messageGameObject;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setMessage(string message)
    {
        this.message = message;
        var children = this.gameObject.GetComponentsInChildren<Transform>();
        foreach (var child in children)
        {
            if (child.gameObject.name == "Name")
            {
                child.gameObject.GetComponent<TMP_Text>().text = playerColor == Color.green ? "Green" : "Hmm";
                child.gameObject.GetComponent<TMP_Text>().color = playerColor;
            }
            if (child.gameObject.name == "Message")
            {
                child.gameObject.GetComponent<TMP_Text>().text = message;

               // this.gameObject.transform.localScale = new Vector2(this.gameObject.transform.localScale.x, child.gameObject.transform.localScale.y);
            }
        }
    } 

    public void setPlayerColor(Color playerColor)
    {
        this.playerColor = playerColor;
    }
}
