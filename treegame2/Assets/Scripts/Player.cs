using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Player : NetworkBehaviour
{
    [SerializeField]
    enum Role
    {
        hacker,
        member
    }

    [SerializeField,SyncVar]
    private Role role;

    [SerializeField,SyncVar]
    private Color color;

    [SyncVar]
    private int playerID;

    public SpriteRenderer sprite;

    // Start is called before the first frame update
    void Start()
    {
        sprite.color = this.color;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A)) {
            this.transform.position += new Vector3(0.5f, 0f, 0f);
        }
    }

    public Color getColor()
    {
        return this.color;
    }

    public void setColor(Color color)
    {
        this.color = color;
    }

    public int GetPlayerID() {
        return this.playerID;
    }

    public void SetPlayerID(int id) {
        this.playerID = id;
    }
}
