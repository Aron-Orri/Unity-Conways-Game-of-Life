using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private bool isAlive;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }


    public void SetAlive(bool alive)
    {
        if (isAlive == alive) return;

        isAlive = alive;
        if (isAlive) spriteRenderer.color = Color.black;
        else spriteRenderer.color = Color.white;
    }

}
