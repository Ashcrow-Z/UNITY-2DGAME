using UnityEngine;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour
{
    public const int MAP_SIZE = 23;

    [Header("Prefabs")]
    public GameObject destructibleWallPrefab;
    public GameObject indestructibleWallPrefab;

    [Header("Generation Settings")]
    [Range(0.1f, 0.3f)]
    public float obstacleDensity = 0.18f;
    [Range(0f, 1f)]
    public float destructibleRatio = 0.6f;

    private int[,] grid; // 0=empty, 1=indestructible, 2=destructible
    private List<GameObject> spawnedObjects = new List<GameObject>();
    private List<Vector2Int> emptyPositions = new List<Vector2Int>();

    public void GenerateMap()
    {
        ClearMap();
        grid = new int[MAP_SIZE, MAP_SIZE];
        emptyPositions.Clear();

        PlaceBoundaryWalls();
        PlaceInteriorObstacles();
        EnsureReachability();
        InstantiateMap();
        CollectEmptyPositions();
    }

    void ClearMap()
    {
        foreach (var obj in spawnedObjects)
        {
            if (obj != null) Destroy(obj);
        }
        spawnedObjects.Clear();
    }

    void PlaceBoundaryWalls()
    {
        for (int x = 0; x < MAP_SIZE; x++)
        {
            for (int y = 0; y < MAP_SIZE; y++)
            {
                if (x == 0 || x == MAP_SIZE - 1 || y == 0 || y == MAP_SIZE - 1)
                {
                    grid[x, y] = 1;
                }
            }
        }
    }

    void PlaceInteriorObstacles()
    {
        Vector2Int playerSpawn = new Vector2Int(2, 2);

        for (int x = 1; x < MAP_SIZE - 1; x++)
        {
            for (int y = 1; y < MAP_SIZE - 1; y++)
            {
                if (Mathf.Abs(x - playerSpawn.x) <= 1 && Mathf.Abs(y - playerSpawn.y) <= 1)
                    continue;

                if (Random.value < obstacleDensity)
                {
                    grid[x, y] = Random.value < destructibleRatio ? 2 : 1;
                }
            }
        }
    }

    void EnsureReachability()
    {
        Vector2Int start = new Vector2Int(2, 2);
        bool[,] visited = new bool[MAP_SIZE, MAP_SIZE];
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        queue.Enqueue(start);
        visited[start.x, start.y] = true;

        int[] dx = { 0, 0, 1, -1 };
        int[] dy = { 1, -1, 0, 0 };

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            for (int i = 0; i < 4; i++)
            {
                int nx = current.x + dx[i];
                int ny = current.y + dy[i];
                if (nx >= 1 && nx < MAP_SIZE - 1 && ny >= 1 && ny < MAP_SIZE - 1
                    && !visited[nx, ny] && grid[nx, ny] == 0)
                {
                    visited[nx, ny] = true;
                    queue.Enqueue(new Vector2Int(nx, ny));
                }
            }
        }

        int emptyCount = 0;
        int reachableCount = 0;
        for (int x = 1; x < MAP_SIZE - 1; x++)
        {
            for (int y = 1; y < MAP_SIZE - 1; y++)
            {
                if (grid[x, y] == 0)
                {
                    emptyCount++;
                    if (visited[x, y]) reachableCount++;
                }
            }
        }

        if (reachableCount < emptyCount * 0.7f)
        {
            for (int x = 1; x < MAP_SIZE - 1; x++)
            {
                for (int y = 1; y < MAP_SIZE - 1; y++)
                {
                    if (grid[x, y] != 0 && !visited[x, y] && grid[x, y] == 2)
                    {
                        if (Random.value < 0.5f)
                            grid[x, y] = 0;
                    }
                }
            }
        }
    }

    void InstantiateMap()
    {
        Transform mapParent = new GameObject("Map").transform;

        GameObject floor = new GameObject("Floor");
        floor.transform.SetParent(mapParent);
        SpriteRenderer floorSR = floor.AddComponent<SpriteRenderer>();
        floorSR.sprite = SpriteGenerator.CreateSquareSprite(32, new Color(0.15f, 0.15f, 0.2f));
        floor.transform.position = new Vector3(0, 0, 0);
        floor.transform.localScale = new Vector3(MAP_SIZE, MAP_SIZE, 1);
        floorSR.sortingOrder = -1;
        spawnedObjects.Add(floor);

        for (int x = 0; x < MAP_SIZE; x++)
        {
            for (int y = 0; y < MAP_SIZE; y++)
            {
                Vector3 pos = GridToWorld(x, y);

                if (grid[x, y] == 1)
                {
                    GameObject wall = Instantiate(indestructibleWallPrefab, pos, Quaternion.identity, mapParent);
                    wall.SetActive(true);
                    spawnedObjects.Add(wall);
                }
                else if (grid[x, y] == 2)
                {
                    GameObject wall = Instantiate(destructibleWallPrefab, pos, Quaternion.identity, mapParent);
                    wall.SetActive(true);
                    spawnedObjects.Add(wall);
                }
            }
        }
    }

    void CollectEmptyPositions()
    {
        emptyPositions.Clear();
        for (int x = 1; x < MAP_SIZE - 1; x++)
        {
            for (int y = 1; y < MAP_SIZE - 1; y++)
            {
                if (grid[x, y] == 0)
                {
                    emptyPositions.Add(new Vector2Int(x, y));
                }
            }
        }
    }

    public Vector2 GetPlayerSpawnPosition()
    {
        return GridToWorld(2, 2);
    }

    public Vector2 GetRandomEmptyPosition()
    {
        if (emptyPositions.Count == 0) return GridToWorld(MAP_SIZE / 2, MAP_SIZE / 2);

        int index = Random.Range(0, emptyPositions.Count);
        Vector2Int cell = emptyPositions[index];
        return GridToWorld(cell.x, cell.y);
    }

    public Vector3 GridToWorld(int x, int y)
    {
        float offset = MAP_SIZE / 2f;
        return new Vector3(x - offset + 0.5f, y - offset + 0.5f, 0);
    }

    public Vector2Int WorldToGrid(Vector3 worldPos)
    {
        float offset = MAP_SIZE / 2f;
        int x = Mathf.RoundToInt(worldPos.x + offset - 0.5f);
        int y = Mathf.RoundToInt(worldPos.y + offset - 0.5f);
        return new Vector2Int(x, y);
    }

    public bool IsWalkable(int x, int y)
    {
        if (x < 0 || x >= MAP_SIZE || y < 0 || y >= MAP_SIZE) return false;
        return grid[x, y] == 0;
    }

    public int[,] GetGrid()
    {
        return grid;
    }

    public void OnWallDestroyed(Vector2Int gridPos)
    {
        if (gridPos.x >= 0 && gridPos.x < MAP_SIZE && gridPos.y >= 0 && gridPos.y < MAP_SIZE)
        {
            grid[gridPos.x, gridPos.y] = 0;
            emptyPositions.Add(gridPos);
        }
    }
}
