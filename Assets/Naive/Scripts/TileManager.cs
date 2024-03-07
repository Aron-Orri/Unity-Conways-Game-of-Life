using UnityEngine;

public class TileManager : MonoBehaviour
{
    private readonly uint WIDTH=128, HEIGHT=128;
    [SerializeField] private GameObject tilePrefab;
    private SpriteRenderer[,] tiles;
    private bool[,] conwayCells;


    private void Start()
    {
        if (tilePrefab == null)
        {
            Debug.LogError("Tile prefab not selected");
            return;
        }

        tiles = new SpriteRenderer[WIDTH,HEIGHT];
        conwayCells = new bool[WIDTH,HEIGHT];

        for (int i = 0; i < WIDTH; i++)
        {
            for (int j = 0; j < HEIGHT; j++)
            {
                tiles[i, j] = Instantiate(tilePrefab, new Vector3(i,j,0), Quaternion.identity).GetComponent<SpriteRenderer>();
                conwayCells[i, j] = 0 == Random.Range(0, 2);
            }
        }
    }

    private void Update()
    {
        for (int i = 0;i < WIDTH;i++)
        {
            for(int j = 0;j < HEIGHT;j++) 
            {
                if (conwayCells[i,j])
                {
                    tiles[i, j].GetComponent<SpriteRenderer>().color = Color.black;
                } else
                {
                    tiles[i, j].GetComponent<SpriteRenderer>().color = Color.white;
                }
            }
        }
    }
}
