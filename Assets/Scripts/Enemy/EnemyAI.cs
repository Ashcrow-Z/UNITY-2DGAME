using UnityEngine;
using System.Collections.Generic;

public class EnemyAI : MonoBehaviour
{
    public bool usePathfinding = true;
    public float pathUpdateInterval = 0.5f;
    public float waypointReachDistance = 0.3f;

    private EnemyBase enemyBase;
    private List<Vector2> currentPath = new List<Vector2>();
    private int currentWaypointIndex;
    private float pathUpdateTimer;
    private Rigidbody2D rb;
    private Transform playerTransform;
    private bool isChasing;

    private Vector2 patrolTarget;
    private bool hasPatrolTarget;
    private float patrolWaitTimer;
    private Vector2 spawnPos;

    void Awake()
    {
        enemyBase = GetComponent<EnemyBase>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerTransform = player.transform;
        spawnPos = transform.position;
        PickPatrolTarget();
    }

    public void SetChasing(bool chasing)
    {
        isChasing = chasing;
        if (!chasing)
        {
            currentPath.Clear();
            currentWaypointIndex = 0;
            rb.velocity = Vector2.zero;
            hasPatrolTarget = false;
            patrolWaitTimer = Random.Range(0.5f, 1.5f);
        }
    }

    void Update()
    {
        if (!usePathfinding) return;
        if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameManager.GameState.Playing)
            return;

        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) playerTransform = player.transform;
            return;
        }

        if (isChasing)
            UpdateChase();
        else if (enemyBase.currentState == EnemyBase.AIState.Idle)
            UpdatePatrol();
    }

    void UpdateChase()
    {
        pathUpdateTimer -= Time.deltaTime;
        if (pathUpdateTimer <= 0f)
        {
            UpdateChasePath();
            pathUpdateTimer = pathUpdateInterval;
        }
        FollowPath();
    }

    void UpdatePatrol()
    {
        if (!hasPatrolTarget)
        {
            rb.velocity = Vector2.zero;
            patrolWaitTimer -= Time.deltaTime;
            if (patrolWaitTimer <= 0f)
                PickPatrolTarget();
            return;
        }

        if (currentPath.Count > 0 && currentWaypointIndex < currentPath.Count)
        {
            float distToFinal = Vector2.Distance(transform.position, patrolTarget);
            if (distToFinal < waypointReachDistance)
            {
                hasPatrolTarget = false;
                patrolWaitTimer = Random.Range(enemyBase.patrolWaitMin, enemyBase.patrolWaitMax);
                rb.velocity = Vector2.zero;
                return;
            }
            FollowPath();
        }
        else
        {
            hasPatrolTarget = false;
            patrolWaitTimer = Random.Range(enemyBase.patrolWaitMin, enemyBase.patrolWaitMax);
            rb.velocity = Vector2.zero;
        }
    }

    void PickPatrolTarget()
    {
        if (LevelManager.Instance == null)
        {
            patrolTarget = spawnPos + Random.insideUnitCircle * 2f;
            hasPatrolTarget = true;
            return;
        }

        MapGenerator mapGen = LevelManager.Instance.GetMapGenerator();
        if (mapGen == null)
        {
            patrolTarget = spawnPos + Random.insideUnitCircle * 2f;
            hasPatrolTarget = true;
            return;
        }

        for (int attempt = 0; attempt < 20; attempt++)
        {
            Vector2 randomDir = Random.insideUnitCircle.normalized;
            float dist = Random.Range(enemyBase.patrolDistance * 0.5f, enemyBase.patrolDistance);
            Vector2 candidate = (Vector2)transform.position + randomDir * dist;
            Vector2Int gridPos = mapGen.WorldToGrid(candidate);
            Vector2Int start = mapGen.WorldToGrid(transform.position);

            if (gridPos == start) continue;
            if (!mapGen.IsWalkable(gridPos.x, gridPos.y)) continue;

            List<Vector2> path = FindPath(mapGen, start, gridPos);
            if (path.Count == 0) continue;

            patrolTarget = candidate;
            hasPatrolTarget = true;
            currentPath = path;
            currentWaypointIndex = 0;
            return;
        }

        hasPatrolTarget = false;
        patrolWaitTimer = Random.Range(enemyBase.patrolWaitMin, enemyBase.patrolWaitMax);
    }

    void UpdateChasePath()
    {
        if (LevelManager.Instance == null || playerTransform == null) return;

        MapGenerator mapGen = LevelManager.Instance.GetMapGenerator();
        if (mapGen == null) return;

        Vector2Int start = mapGen.WorldToGrid(transform.position);
        Vector2Int end = mapGen.WorldToGrid(playerTransform.position);

        currentPath = FindPath(mapGen, start, end);
        currentWaypointIndex = 0;
    }

    void FollowPath()
    {
        if (currentPath.Count == 0 || currentWaypointIndex >= currentPath.Count) return;

        Vector2 target = currentPath[currentWaypointIndex];
        Vector2 direction = (target - (Vector2)transform.position).normalized;
        float distance = Vector2.Distance(transform.position, target);

        if (distance < waypointReachDistance)
        {
            currentWaypointIndex++;
            if (currentWaypointIndex >= currentPath.Count) return;
            target = currentPath[currentWaypointIndex];
            direction = (target - (Vector2)transform.position).normalized;
        }

        float spd = isChasing ? enemyBase.moveSpeed : enemyBase.idleMoveSpeed;
        rb.velocity = direction * spd;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90);
    }

    List<Vector2> FindPath(MapGenerator mapGen, Vector2Int start, Vector2Int end)
    {
        if (!mapGen.IsWalkable(end.x, end.y))
        {
            end = FindNearestWalkable(mapGen, end);
        }

        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        Dictionary<Vector2Int, float> gScore = new Dictionary<Vector2Int, float>();
        Dictionary<Vector2Int, float> fScore = new Dictionary<Vector2Int, float>();
        HashSet<Vector2Int> closedSet = new HashSet<Vector2Int>();

        var openSet = new SortedSet<(float f, Vector2Int pos)>(
            Comparer<(float f, Vector2Int pos)>.Create((a, b) =>
            {
                int cmp = a.f.CompareTo(b.f);
                if (cmp != 0) return cmp;
                cmp = a.pos.x.CompareTo(b.pos.x);
                if (cmp != 0) return cmp;
                return a.pos.y.CompareTo(b.pos.y);
            }));

        gScore[start] = 0;
        fScore[start] = Heuristic(start, end);
        openSet.Add((fScore[start], start));

        int[] dx = { 0, 0, 1, -1 };
        int[] dy = { 1, -1, 0, 0 };

        int maxIterations = 500;
        int iterations = 0;

        while (openSet.Count > 0 && iterations < maxIterations)
        {
            iterations++;
            var current = openSet.Min;
            openSet.Remove(current);
            Vector2Int currentPos = current.pos;

            if (currentPos == end)
            {
                return ReconstructPath(mapGen, cameFrom, currentPos);
            }

            closedSet.Add(currentPos);

            for (int i = 0; i < 4; i++)
            {
                Vector2Int neighbor = new Vector2Int(currentPos.x + dx[i], currentPos.y + dy[i]);

                if (!mapGen.IsWalkable(neighbor.x, neighbor.y) || closedSet.Contains(neighbor))
                    continue;

                float tentativeG = gScore[currentPos] + 1f;

                if (!gScore.ContainsKey(neighbor) || tentativeG < gScore[neighbor])
                {
                    cameFrom[neighbor] = currentPos;
                    gScore[neighbor] = tentativeG;
                    float f = tentativeG + Heuristic(neighbor, end);
                    fScore[neighbor] = f;
                    openSet.Add((f, neighbor));
                }
            }
        }

        return new List<Vector2>();
    }

    Vector2Int FindNearestWalkable(MapGenerator mapGen, Vector2Int target)
    {
        for (int radius = 1; radius < 5; radius++)
        {
            for (int ddx = -radius; ddx <= radius; ddx++)
            {
                for (int ddy = -radius; ddy <= radius; ddy++)
                {
                    Vector2Int check = new Vector2Int(target.x + ddx, target.y + ddy);
                    if (mapGen.IsWalkable(check.x, check.y))
                        return check;
                }
            }
        }
        return target;
    }

    float Heuristic(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    List<Vector2> ReconstructPath(MapGenerator mapGen, Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int current)
    {
        List<Vector2> path = new List<Vector2>();
        while (cameFrom.ContainsKey(current))
        {
            path.Add(mapGen.GridToWorld(current.x, current.y));
            current = cameFrom[current];
        }
        path.Reverse();
        return path;
    }
}
