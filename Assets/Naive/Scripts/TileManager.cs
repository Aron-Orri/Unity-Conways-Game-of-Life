using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    private readonly uint WIDTH=64, HEIGHT=64;
    [SerializeField] private GameObject tilePrefab;
    private GameObject[,] tiles;


    private void Start()
    {
        if (tilePrefab == null)
        {
            Debug.LogError("Tile prefab not selected");
            return;
        }

        tiles = new GameObject[WIDTH,HEIGHT];

        for (int i = 0; i < WIDTH; i++)
        {
            for (int j = 0; j < HEIGHT; j++)
            {
                tiles[i, j] = Instantiate(tilePrefab, new Vector3(i,j,0), Quaternion.identity);
            }
        }
    }
}
