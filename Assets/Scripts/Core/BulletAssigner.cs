using UnityEngine;

public class BulletAssigner : MonoBehaviour
{
    void Start()
    {
        AssignBulletPrefabs();
    }

    public static void AssignBulletPrefabs()
    {
        if (GameSetup.Instance == null) return;

        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null && player.bulletPrefab == null)
        {
            player.bulletPrefab = GameSetup.Instance.bulletPrefab;
        }

        EnemyBase[] enemies = FindObjectsOfType<EnemyBase>();
        foreach (var enemy in enemies)
        {
            if (enemy.bulletPrefab == null)
            {
                enemy.bulletPrefab = GameSetup.Instance.bulletPrefab;
            }
        }
    }
}
