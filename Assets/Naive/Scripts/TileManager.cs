using UnityEngine;

public class TileManager : MonoBehaviour
{
    [SerializeField] private uint WIDTH=500;
    [SerializeField] private uint HEIGHT=500;
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private GameObject mainCamera;
    [Range(0f, 1f)]
    [SerializeField] private float tickInterval = 0.1f;
    [SerializeField] private bool enableWrapping = false;

    private Tile[,] tiles;
    private bool[,] conwayCells, cacheCells;
    private float lastTime;
    private bool isPaused;



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
                // Spawn tiles shifted by .5 so that the lower left corner of the first tile is in (0,0)
                tiles[i, j] = Instantiate(tilePrefab, new Vector3(i+.5f,j+.5f,0), Quaternion.identity).GetComponent<Tile>();
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

    private int CountAliveNeighborsWrapping(int x, int y)
    {
        int count = 0;

        // Lower left (x-1, y-1)
        if (0 < x && 0 < y) count += cacheCells[x - 1, y - 1] ? 1 : 0; // Default
        else if (x == 0 && 0 < y) count += cacheCells[WIDTH - 1, y - 1] ? 1 : 0; // Wrap x
        else if (0 < x && y == 0) count += cacheCells[x - 1, HEIGHT - 1] ? 1 : 0; // Wrap y
        else if (x == 0 && y == 0) count += cacheCells[WIDTH - 1, HEIGHT - 1] ? 1 : 0; // Wrap both
        else Debug.LogError("Unexpected coordinates");
        
        // Middle left (x-1, y)
        if (0 < x) count += cacheCells[x-1, y] ? 1 : 0; // Default
        else if (x == 0) count += cacheCells[WIDTH-1, y] ? 1 : 0; // Wrap x
        else Debug.LogError("Unexpected coordinates");

        // Upper left (x-1, y+1)
        if (0 < x && y < HEIGHT - 1) count += cacheCells[x - 1, y + 1] ? 1 : 0; // Default
        else if (x == 0 && y < HEIGHT - 1) count += cacheCells[WIDTH - 1, y + 1] ? 1 : 0; // Wrap x
        else if (0 < x && y == HEIGHT - 1) count += cacheCells[x - 1, 0] ? 1 : 0; // Wrap y
        else if (x == 0 && y == HEIGHT - 1) count += cacheCells[WIDTH - 1, 0] ? 1 : 0; // Wrap both
        else Debug.LogError("Unexpected coordinates");

        // Top middle (x, y-1)
        if (0 < y) count += cacheCells[x, y-1] ? 1 : 0; // Default
        else if (y == 0) count += cacheCells[x,HEIGHT-1] ? 1 : 0; // Wrap y
        else Debug.LogError("Unexpected coordinates");
        
        // Lower middle (x, y+1)
        if (y < HEIGHT-1) count += cacheCells[x, y+1] ? 1 : 0; // Default
        else if (y == HEIGHT-1) count += cacheCells[x, 0] ? 1 : 0; // Wrap y
        else Debug.LogError("Unexpected coordinates");

        // Lower right (x+1, y-1)
        if (x < WIDTH-1 && 0 < y) count += cacheCells[x + 1, y - 1] ? 1 : 0; // Default
        else if (x == WIDTH-1 && 0 < y) count += cacheCells[0, y-1] ? 1 : 0; // Wrap x
        else if (x < WIDTH-1 && y == 0) count += cacheCells[x+1, HEIGHT-1] ? 1 : 0; // Wrap y
        else if (x == WIDTH-1 && y == 0) count += cacheCells[0, HEIGHT - 1] ? 1 : 0; // Wrap both
        else Debug.LogError("Unexpected coordinates");
        
        // Middle right (x+1, y)
        if (x < WIDTH-1) count += cacheCells[x+1, y] ? 1 : 0; // Default
        else if (x == WIDTH-1) count += cacheCells[0, y] ? 1 : 0; // Wrap x
        else Debug.LogError("Unexpected coordinates");

        // Upper right (x+1, y-1)
        if (x < WIDTH-1 && y < HEIGHT - 1) count += cacheCells[x + 1, y + 1] ? 1 : 0; // Default
        else if (x == WIDTH-1 && y < HEIGHT - 1) count += cacheCells[0, y + 1] ? 1 : 0; // Wrap x
        else if (x < WIDTH-1 && y == HEIGHT - 1) count += cacheCells[x + 1, 0] ? 1 : 0; // Wrap y
        else if (x == WIDTH-1 && y == HEIGHT - 1) count += cacheCells[0, 0] ? 1 : 0; // Wrap both
        else Debug.LogError("Unexpected coordinates");

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
                int aliveNeighbors = 0;
                if (!enableWrapping) aliveNeighbors = CountAliveNeighbors(i, j);
                else aliveNeighbors = CountAliveNeighborsWrapping(i, j);

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

    private void HandleLeftMouseClick()
    {
        Vector3 pos = mainCamera.GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);
        conwayCells[(int)pos.x, (int)pos.y] = !conwayCells[(int)pos.x, (int)pos.y];
        UpdateTiles();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) isPaused = !isPaused;
        if (Input.GetMouseButtonDown(0)) HandleLeftMouseClick();
        if (isPaused)
        {
            return;
        }
        if (Time.time - lastTime < tickInterval) return;
        lastTime = Time.time;
        CacheCells();
        UpdateConway();
        UpdateTiles();
    }
}
