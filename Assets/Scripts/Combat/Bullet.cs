using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    public float lifetime = 3f;

    private Vector2 direction;
    private float speed;
    private int damage;
    private bool isPlayerBullet;
    private float lifetimeTimer;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    public void Initialize(Vector2 dir, float spd, int dmg, bool fromPlayer)
    {
        direction = dir.normalized;
        speed = spd;
        damage = dmg;
        isPlayerBullet = fromPlayer;
        lifetimeTimer = lifetime;
        gameObject.SetActive(true);

        int layer = fromPlayer
            ? LayerMask.NameToLayer("PlayerBullet")
            : LayerMask.NameToLayer("EnemyBullet");
        if (layer >= 0) gameObject.layer = layer;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            sr.color = fromPlayer ? new Color(0.4f, 1f, 1f) : new Color(1f, 0.3f, 0.15f);
    }

    void Update()
    {
        lifetimeTimer -= Time.deltaTime;
        if (lifetimeTimer <= 0f)
        {
            Deactivate();
        }
    }

    void FixedUpdate()
    {
        rb.velocity = direction * speed;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Obstacle obstacle = other.GetComponent<Obstacle>();
        if (obstacle != null)
        {
            if (obstacle.isDestructible)
            {
                obstacle.TakeDamage(damage);
            }
            Deactivate();
            return;
        }

        if (isPlayerBullet)
        {
            EnemyBase enemy = other.GetComponent<EnemyBase>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Deactivate();
                return;
            }
        }
        else
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                Deactivate();
                return;
            }
        }
    }

    void Deactivate()
    {
        rb.velocity = Vector2.zero;
        if (BulletPool.Instance != null)
        {
            BulletPool.Instance.ReturnBullet(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
