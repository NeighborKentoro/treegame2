using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MessageInput : MonoBehaviour
{
    TMP_InputField inputField;

    private int messagingAs;

    private void Start()
    {
    }

    private void Update()
    {
        inputField.ActivateInputField();
    }

    private void OnEnable()
    {

        inputField = gameObject.GetComponent<TMP_InputField>();
        inputField.onEndEdit.AddListener(SendUserMessage);
    }

    private void OnDisable()
    {
        inputField.onEndEdit.RemoveListener(SendUserMessage);
    }

    private void SendUserMessage(string message)
    {
        message = message.Trim();
        if (string.IsNullOrEmpty(message))
        {
            return;
        }
        EventManager.UserSendMessage(message, messagingAs);
        inputField.text = "";
    }

    public void setMessagingAs(int playerID, Color color)
    {
        this.messagingAs = playerID;
        GameObject placeholder = GameObject.Find("MessageInputPlaceholder");
        placeholder.GetComponent<TMP_Text>().text = "Messaging as player " + playerID;

        GameObject messageInputText = GameObject.Find("MessageInputText");
        messageInputText.GetComponent<TMP_Text>().faceColor = color;
    }

    public void DisableInputField() {
        inputField.interactable = false;
    }
}
