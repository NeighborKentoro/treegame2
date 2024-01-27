using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    enum Role
    {
        hacker,
        member
    }

    [SerializeField]
    private Role role;

    [SerializeField]
    private Color color;

    // Start is called before the first frame update
    void Start()
    {
        color = Color.green;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Color getColor()
    {
        return this.color;
    }

    public void setColor(Color color)
    {
        this.color = color;
    }
}
