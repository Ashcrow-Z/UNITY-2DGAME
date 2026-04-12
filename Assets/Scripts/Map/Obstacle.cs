using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public bool isDestructible = true;
    public int health = 3;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;
    }

    public void TakeDamage(int damage)
    {
        if (!isDestructible) return;

        health -= damage;

        if (spriteRenderer != null)
        {
            float t = (float)health / 3f;
            spriteRenderer.color = Color.Lerp(Color.white, originalColor, t);
        }

        if (health <= 0)
        {
            MapGenerator mapGen = FindObjectOfType<MapGenerator>();
            if (mapGen != null)
            {
                Vector2Int gridPos = mapGen.WorldToGrid(transform.position);
                mapGen.OnWallDestroyed(gridPos);
            }

            if (VFXManager.Instance != null)
                VFXManager.Instance.SpawnExplosion(transform.position, new Color(0.6f, 0.4f, 0.2f), 0.7f);

            Destroy(gameObject);
        }
    }
}
