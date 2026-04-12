using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("Prefabs")]
    public GameObject playerPrefab;
    public GameObject enemyNormalPrefab;
    public GameObject enemyReinforcedPrefab;
    public GameObject destructibleWallPrefab;
    public GameObject indestructibleWallPrefab;

    [Header("Level 1 Settings")]
    public int level1EnemyCount = 10;

    [Header("Level 2 Settings")]
    public int level2TotalEnemies = 20;
    public int level2InitialSpawn = 10;
    public float respawnDelay = 2f;

    private MapGenerator mapGenerator;
    private int enemiesAlive;
    private int enemiesSpawned;
    private int totalEnemiesForLevel;
    private int enemiesDefeated;
    private GameObject playerInstance;
    private List<GameObject> activeEnemies = new List<GameObject>();

    public int EnemiesDefeated => enemiesDefeated;
    public int TotalEnemiesForLevel => totalEnemiesForLevel;

    void Awake()
    {
        Instance = this;
        mapGenerator = GetComponent<MapGenerator>();
    }

    void Start()
    {
        int level = GameManager.Instance != null ? GameManager.Instance.CurrentLevel : 1;
        SetupLevel(level);
    }

    void SetupLevel(int level)
    {
        mapGenerator.GenerateMap();

        Vector2 spawnPos = mapGenerator.GetPlayerSpawnPosition();
        playerInstance = Instantiate(playerPrefab, spawnPos, Quaternion.identity);
        playerInstance.SetActive(true);

        PlayerHealth playerHealth = playerInstance.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.ResetHealth();
            playerHealth.SetRespawnPosition(spawnPos);
        }

        enemiesDefeated = 0;

        if (level == 1)
        {
            totalEnemiesForLevel = level1EnemyCount;
            enemiesSpawned = 0;
            SpawnEnemies(level1EnemyCount, false);
        }
        else
        {
            totalEnemiesForLevel = level2TotalEnemies;
            enemiesSpawned = 0;
            SpawnEnemies(level2InitialSpawn, true);

            if (GetComponent<PropSpawner>() != null)
                GetComponent<PropSpawner>().StartSpawning();
        }

        GameManager.Instance.BeginPlaying();
    }

    void SpawnEnemies(int count, bool includeReinforced)
    {
        int level = GameManager.Instance != null ? GameManager.Instance.CurrentLevel : 1;
        for (int i = 0; i < count; i++)
        {
            Vector2 pos = mapGenerator.GetRandomEmptyPosition();
            GameObject prefab;

            if (includeReinforced && i % 3 == 0)
                prefab = enemyReinforcedPrefab;
            else
                prefab = enemyNormalPrefab;

            GameObject enemy = Instantiate(prefab, pos, Quaternion.identity);
            enemy.SetActive(true);

            if (level == 2)
                ApplyLevel2Stats(enemy);

            activeEnemies.Add(enemy);
            enemiesSpawned++;
        }
        enemiesAlive = activeEnemies.Count;
    }

    void ApplyLevel2Stats(GameObject enemy)
    {
        EnemyBase eb = enemy.GetComponent<EnemyBase>();
        if (eb == null) return;
        eb.moveSpeed = 3f;
        eb.idleMoveSpeed = 1.5f;
    }

    public void OnEnemyDefeated(GameObject enemy, int scoreValue)
    {
        activeEnemies.Remove(enemy);
        enemiesAlive--;
        enemiesDefeated++;
        GameManager.Instance.AddScore(scoreValue);

        if (enemiesDefeated >= totalEnemiesForLevel)
        {
            GameManager.Instance.OnAllEnemiesDefeated();
            return;
        }

        if (GameManager.Instance.CurrentLevel == 2 && enemiesSpawned < totalEnemiesForLevel)
        {
            StartCoroutine(RespawnEnemy());
        }
    }

    IEnumerator RespawnEnemy()
    {
        yield return new WaitForSeconds(respawnDelay);

        if (enemiesSpawned >= totalEnemiesForLevel) yield break;

        Vector2 pos = mapGenerator.GetRandomEmptyPosition();
        GameObject prefab;

        if (Random.value < 0.33f)
            prefab = enemyReinforcedPrefab;
        else
            prefab = enemyNormalPrefab;

        GameObject enemy = Instantiate(prefab, pos, Quaternion.identity);
        enemy.SetActive(true);
        ApplyLevel2Stats(enemy);
        activeEnemies.Add(enemy);
        enemiesSpawned++;
        enemiesAlive++;
    }

    public Vector2 GetPlayerPosition()
    {
        if (playerInstance != null)
            return playerInstance.transform.position;
        return Vector2.zero;
    }

    public MapGenerator GetMapGenerator()
    {
        return mapGenerator;
    }
}
