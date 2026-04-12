using UnityEngine;
using System.Collections.Generic;

public class BulletPool : MonoBehaviour
{
    public static BulletPool Instance { get; private set; }

    public GameObject bulletPrefab;
    public int poolSize = 30;

    private Queue<GameObject> pool = new Queue<GameObject>();
    private bool initialized;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (bulletPrefab == null && GameSetup.Instance != null)
            bulletPrefab = GameSetup.Instance.bulletPrefab;

        InitializePool();
    }

    void InitializePool()
    {
        if (initialized || bulletPrefab == null) return;

        for (int i = 0; i < poolSize; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab);
            bullet.SetActive(false);
            pool.Enqueue(bullet);
        }
        initialized = true;
    }

    public GameObject GetBullet()
    {
        if (!initialized) InitializePool();

        if (pool.Count > 0)
        {
            GameObject bullet = pool.Dequeue();
            bullet.SetActive(true);
            return bullet;
        }

        GameObject newBullet = Instantiate(bulletPrefab);
        newBullet.SetActive(true);
        return newBullet;
    }

    public void ReturnBullet(GameObject bullet)
    {
        bullet.SetActive(false);
        pool.Enqueue(bullet);
    }
}
