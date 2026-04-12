using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PropSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject firepowerPropPrefab;
    public GameObject healthPackPropPrefab;

    [Header("Settings")]
    public float spawnInterval = 18f;
    public int maxPropsOnMap = 2;

    private List<GameObject> activeProps = new List<GameObject>();
    private bool isSpawning;

    public void StartSpawning()
    {
        if (!isSpawning)
        {
            isSpawning = true;
            StartCoroutine(SpawnLoop());
        }
    }

    public void StopSpawning()
    {
        isSpawning = false;
        StopAllCoroutines();
    }

    IEnumerator SpawnLoop()
    {
        while (isSpawning)
        {
            yield return new WaitForSeconds(spawnInterval);

            activeProps.RemoveAll(p => p == null);

            if (activeProps.Count < maxPropsOnMap)
            {
                SpawnRandomProp();
            }
        }
    }

    void SpawnRandomProp()
    {
        if (LevelManager.Instance == null) return;

        MapGenerator mapGen = LevelManager.Instance.GetMapGenerator();
        if (mapGen == null) return;

        Vector2 pos = mapGen.GetRandomEmptyPosition();

        GameObject prefab = Random.value < 0.5f ? firepowerPropPrefab : healthPackPropPrefab;
        if (prefab == null) return;

        GameObject prop = Instantiate(prefab, pos, Quaternion.identity);
        prop.SetActive(true);
        activeProps.Add(prop);
    }
}
