using UnityEngine;

public class TileManager : MonoBehaviour
{
    [SerializeField] private uint WIDTH=500;
    [SerializeField] private uint HEIGHT=500;
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private GameObject mainCamera;
    [Range(0f, 1f)]
    [SerializeField] private float tickInterval = 0.1f;
    private Tile[,] tiles;
    private bool[,] conwayCells, cacheCells;
    private float lastTime;



    private void Start()
    {
        if (tilePrefab == null)
        {
            Debug.LogError("Tile prefab not selected");
            return;
        }

        tiles = new Tile[WIDTH,HEIGHT];
        conwayCells = new bool[WIDTH,HEIGHT];
        cacheCells = new bool[WIDTH,HEIGHT];

        for (int i = 0; i < WIDTH; i++)
        {
            for (int j = 0; j < HEIGHT; j++)
            {
                tiles[i, j] = Instantiate(tilePrefab, new Vector3(i,j,0), Quaternion.identity).GetComponent<Tile>();
                conwayCells[i, j] = 0 == Random.Range(0, 2);
            }
        }

        lastTime = Time.time;

        mainCamera.transform.position = new Vector3(WIDTH/2, HEIGHT/2, mainCamera.transform.position.z);
        mainCamera.GetComponent<Camera>().orthographicSize = WIDTH/2;
    }


    private int CountAliveNeighbors(int x, int y)
    {
        int count = 0;

        if (x > 0)
        {
            if (y > 0 && cacheCells[x-1, y-1]) count++;
            if (cacheCells[x-1, y]) count++;
            if (y < HEIGHT-1 && cacheCells[x-1,y+1]) count++;
        }

        if (x < WIDTH - 1)
        {
            if (y > 0 && cacheCells[x+1, y-1]) count++;
            if (cacheCells[x+1, y]) count++;
            if (y < HEIGHT-1 && cacheCells[x+1,y+1]) count++;
        }

        if (y > 0 && cacheCells[x,y-1]) count++;
        if (y < HEIGHT-1 && cacheCells[x,y+1]) count++;

        return count;
    }

    private void CacheCells()
    {
        for (int i = 0; i < WIDTH; i++)
        {
            for (int j = 0;j < HEIGHT; j++)
            {
                cacheCells[i, j] = conwayCells[i, j];
            }
        }
    }

    private void UpdateConway()
    {
        for (int i = 0;i < WIDTH;i++)
        {
            for (int j = 0;j < HEIGHT;j++)
            {
                int aliveNeighbors = CountAliveNeighbors(i,j);

                if (!cacheCells[i, j] && aliveNeighbors == 3) conwayCells[i, j] = true;
                else if (aliveNeighbors < 2) conwayCells[i,j] = false;
                else if (aliveNeighbors > 3) conwayCells[i,j] = false;

            }
        }
    }

    private void UpdateTiles()
    {
        for (int i = 0;i < WIDTH;i++)
        {
            for(int j = 0;j < HEIGHT; j++)
            {
                tiles[i, j].SetAlive(conwayCells[i,j]);
            }
        }
    }

    private void Update()
    {
        if (Time.time - lastTime < tickInterval) return;
        lastTime = Time.time;
        CacheCells();
        UpdateConway();
        UpdateTiles();
    }
}
