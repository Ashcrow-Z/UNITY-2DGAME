using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class EnemyBase : MonoBehaviour
{
    public enum AIState { Idle, Chase, Combat }

    [Header("Stats")]
    public int maxHealth = 2;
    public float moveSpeed = 2f;
    public int damage = 1;
    public float attackCooldown = 1.5f;
    public int scoreValue = 10;

    [Header("Combat")]
    public GameObject bulletPrefab;
    public float bulletSpeed = 4f;
    public float detectionRange = 10f;
    public float attackRange = 5f;
    public float keepDistance = 3f;

    [Header("Patrol")]
    public float patrolDistance = 2.5f;
    public float patrolWaitMin = 1f;
    public float patrolWaitMax = 2.5f;
    public float idleMoveSpeed = 1f;

    protected int currentHealth;
    protected float attackTimer;
    protected Rigidbody2D rb;
    protected SpriteRenderer spriteRenderer;
    protected Transform playerTransform;
    private EnemyAI enemyAI;
    private HealthBar healthBar;

    [HideInInspector] public AIState currentState = AIState.Idle;
    private Vector2 patrolTarget;
    private float patrolWaitTimer;
    private bool hasPatrolTarget;
    private Vector2 spawnPosition;
    private Vector2 strafeDir;
    private float strafeSwitchTimer;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentHealth = maxHealth;
    }

    protected virtual void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerTransform = player.transform;

        if (bulletPrefab == null && GameSetup.Instance != null)
            bulletPrefab = GameSetup.Instance.bulletPrefab;

        enemyAI = GetComponent<EnemyAI>();
        spawnPosition = transform.position;

        healthBar = gameObject.AddComponent<HealthBar>();
        healthBar.barColor = Color.red;
        healthBar.yOffset = 0.55f;
        healthBar.barWidth = 0.7f;
        healthBar.Setup(maxHealth, currentHealth, healthBar.barColor);

        PickNewPatrolTarget();
        strafeSwitchTimer = Random.Range(1f, 2.5f);
        strafeDir = Random.value > 0.5f ? Vector2.right : Vector2.left;
    }

    protected virtual void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameManager.GameState.Playing)
            return;

        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                playerTransform = player.transform;
            return;
        }

        attackTimer -= Time.deltaTime;

        float distToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        UpdateAIState(distToPlayer);

        switch (currentState)
        {
            case AIState.Idle:
                HandleIdleState();
                break;
            case AIState.Chase:
                HandleChaseState(distToPlayer);
                break;
            case AIState.Combat:
                HandleCombatState(distToPlayer);
                break;
        }
    }

    void UpdateAIState(float distToPlayer)
    {
        switch (currentState)
        {
            case AIState.Idle:
                if (distToPlayer <= detectionRange)
                {
                    currentState = AIState.Chase;
                    if (enemyAI != null) enemyAI.SetChasing(true);
                }
                break;

            case AIState.Chase:
                if (distToPlayer > detectionRange * 1.3f)
                {
                    currentState = AIState.Idle;
                    if (enemyAI != null) enemyAI.SetChasing(false);
                    PickNewPatrolTarget();
                }
                else if (distToPlayer <= attackRange)
                {
                    currentState = AIState.Combat;
                    if (enemyAI != null) enemyAI.SetChasing(false);
                }
                break;

            case AIState.Combat:
                if (distToPlayer > attackRange * 1.2f)
                {
                    currentState = AIState.Chase;
                    if (enemyAI != null) enemyAI.SetChasing(true);
                }
                else if (distToPlayer > detectionRange * 1.3f)
                {
                    currentState = AIState.Idle;
                    if (enemyAI != null) enemyAI.SetChasing(false);
                    PickNewPatrolTarget();
                }
                break;
        }
    }

    void HandleIdleState()
    {
        if (enemyAI != null && enemyAI.usePathfinding) return;

        if (!hasPatrolTarget)
        {
            rb.velocity = Vector2.zero;
            patrolWaitTimer -= Time.deltaTime;
            if (patrolWaitTimer <= 0f)
                PickNewPatrolTarget();
            return;
        }

        float distToTarget = Vector2.Distance(transform.position, patrolTarget);
        if (distToTarget < 0.3f)
        {
            hasPatrolTarget = false;
            patrolWaitTimer = Random.Range(patrolWaitMin, patrolWaitMax);
            rb.velocity = Vector2.zero;
            return;
        }

        Vector2 dir = (patrolTarget - (Vector2)transform.position).normalized;
        Vector2 avoidance = GetObstacleAvoidance(dir);
        if (avoidance.sqrMagnitude > 0.1f)
        {
            dir = (dir + avoidance).normalized;
        }

        rb.velocity = dir * idleMoveSpeed;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90);
    }

    void HandleChaseState(float distToPlayer)
    {
        bool aiHandlesMovement = enemyAI != null && enemyAI.usePathfinding;
        if (!aiHandlesMovement)
        {
            Vector2 direction = ((Vector2)playerTransform.position - (Vector2)transform.position).normalized;
            MoveInDirection(direction);
        }
    }

    void HandleCombatState(float distToPlayer)
    {
        if (playerTransform == null) return;

        Vector2 toPlayer = ((Vector2)playerTransform.position - (Vector2)transform.position).normalized;

        strafeSwitchTimer -= Time.deltaTime;
        if (strafeSwitchTimer <= 0f)
        {
            strafeDir = -strafeDir;
            strafeSwitchTimer = Random.Range(1f, 2.5f);
        }

        Vector2 perpendicular = new Vector2(-toPlayer.y, toPlayer.x);
        Vector2 moveDir = perpendicular * (strafeDir.x > 0 ? 1f : -1f);

        if (distToPlayer < keepDistance)
            moveDir += -toPlayer * 0.5f;

        if (moveDir.sqrMagnitude > 0.01f)
        {
            moveDir.Normalize();
            Vector2 avoidance = GetObstacleAvoidance(moveDir);
            moveDir = (moveDir + avoidance).normalized;
            rb.velocity = moveDir * moveSpeed * 0.7f;
        }
        else
        {
            rb.velocity = Vector2.zero;
        }

        float aimAngle = Mathf.Atan2(toPlayer.y, toPlayer.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, aimAngle - 90);

        if (attackTimer <= 0f)
        {
            Attack();
            attackTimer = attackCooldown;
        }
    }

    void PickNewPatrolTarget()
    {
        Vector2 randomDir = Random.insideUnitCircle.normalized;
        float dist = Random.Range(patrolDistance * 0.5f, patrolDistance);
        patrolTarget = (Vector2)transform.position + randomDir * dist;
        hasPatrolTarget = true;
    }

    protected virtual void MoveInDirection(Vector2 direction)
    {
        Vector2 avoidance = GetObstacleAvoidance(direction);
        direction = (direction + avoidance).normalized;

        rb.velocity = direction * moveSpeed;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90);
    }

    protected Vector2 GetObstacleAvoidance(Vector2 moveDir)
    {
        Vector2 avoidance = Vector2.zero;
        float rayLength = 1.2f;

        Vector2 right = new Vector2(moveDir.y, -moveDir.x);
        Vector2 left = new Vector2(-moveDir.y, moveDir.x);

        int wallMask = LayerMask.GetMask("Wall");

        RaycastHit2D hitForward = Physics2D.Raycast(transform.position, moveDir, rayLength, wallMask);
        RaycastHit2D hitRight = Physics2D.Raycast(transform.position, (moveDir + right * 0.5f).normalized, rayLength, wallMask);
        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, (moveDir + left * 0.5f).normalized, rayLength, wallMask);

        if (hitForward.collider != null)
        {
            avoidance += (Vector2)hitForward.normal * 1.5f;
        }
        if (hitRight.collider != null)
        {
            avoidance += left * 0.8f;
        }
        if (hitLeft.collider != null)
        {
            avoidance += right * 0.8f;
        }

        return avoidance;
    }

    protected virtual void Attack()
    {
        if (bulletPrefab == null || playerTransform == null) return;

        Vector2 direction = ((Vector2)playerTransform.position - (Vector2)transform.position).normalized;
        Vector3 spawnPos = transform.position + (Vector3)(direction * 0.5f);

        GameObject bulletObj = BulletPool.Instance != null
            ? BulletPool.Instance.GetBullet()
            : Instantiate(bulletPrefab);

        bulletObj.transform.position = spawnPos;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        bulletObj.transform.rotation = Quaternion.Euler(0, 0, angle - 90);

        Bullet bullet = bulletObj.GetComponent<Bullet>();
        if (bullet != null)
        {
            bullet.Initialize(direction, bulletSpeed, damage, false);
        }
    }

    public virtual void TakeDamage(int dmg)
    {
        currentHealth -= dmg;

        if (healthBar != null) healthBar.UpdateBar(currentHealth);

        if (spriteRenderer != null)
        {
            StartCoroutine(FlashDamage());
        }

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(AudioManager.Instance.hitSFX);

        if (currentState == AIState.Idle)
        {
            currentState = AIState.Chase;
            if (enemyAI != null) enemyAI.SetChasing(true);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    System.Collections.IEnumerator FlashDamage()
    {
        if (spriteRenderer == null) yield break;
        Color original = spriteRenderer.color;
        spriteRenderer.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        if (spriteRenderer != null)
            spriteRenderer.color = original;
    }

    protected virtual void Die()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(AudioManager.Instance.explosionSFX);

        if (VFXManager.Instance != null)
        {
            Color c = spriteRenderer != null ? spriteRenderer.color : Color.red;
            VFXManager.Instance.SpawnExplosion(transform.position, c);
        }

        if (LevelManager.Instance != null)
            LevelManager.Instance.OnEnemyDefeated(gameObject, scoreValue);

        Destroy(gameObject);
    }
}
