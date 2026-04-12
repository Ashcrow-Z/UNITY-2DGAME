using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 3f;
    public float sprintSpeed = 5f;
    public float maxStamina = 100f;
    public float staminaDrainRate = 40f;
    public float staminaRegenRate = 25f;
    public float staminaRegenDelay = 0.5f;

    [Header("Combat")]
    public float attackCooldown = 1f;
    public GameObject bulletPrefab;
    public float bulletSpeed = 8f;
    public int bulletDamage = 1;
    public Transform firePoint;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private float attackTimer;
    private float currentStamina;
    private float regenDelayTimer;
    private bool isSprinting;
    private SpriteRenderer spriteRenderer;
    private Camera mainCam;
    private Transform visualChild;
    public SpriteRenderer VisualSpriteRenderer { get; private set; }
    public float StaminaPercent => currentStamina / maxStamina;

    public bool HasFirepowerBoost { get; set; }
    private float firepowerBoostTimer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        spriteRenderer = GetComponent<SpriteRenderer>();

        SetupVisualChild();
    }

    void SetupVisualChild()
    {
        Transform existingVisual = transform.Find("Visual");
        if (existingVisual != null)
        {
            visualChild = existingVisual;
            VisualSpriteRenderer = existingVisual.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = null;
                spriteRenderer.enabled = false;
            }
            return;
        }

        if (spriteRenderer == null || spriteRenderer.sprite == null) return;

        visualChild = new GameObject("Visual").transform;
        visualChild.SetParent(transform, false);
        visualChild.localPosition = Vector3.zero;

        SpriteRenderer childSR = visualChild.gameObject.AddComponent<SpriteRenderer>();
        childSR.sprite = spriteRenderer.sprite;
        childSR.color = spriteRenderer.color;
        childSR.sortingOrder = spriteRenderer.sortingOrder;
        childSR.sortingLayerID = spriteRenderer.sortingLayerID;

        VisualSpriteRenderer = childSR;

        spriteRenderer.sprite = null;
        spriteRenderer.enabled = false;
    }

    void Start()
    {
        mainCam = Camera.main;
        currentStamina = maxStamina;
        if (bulletPrefab == null && GameSetup.Instance != null)
            bulletPrefab = GameSetup.Instance.bulletPrefab;
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameManager.GameState.Playing)
            return;

        HandleMovementInput();
        HandleSprint();
        HandleAiming();
        HandleAttack();
        HandleFirepowerBoost();
    }

    void FixedUpdate()
    {
        if (GameManager.Instance != null && GameManager.Instance.CurrentState != GameManager.GameState.Playing)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        float speed = isSprinting ? sprintSpeed : moveSpeed;
        rb.velocity = moveInput * speed;
    }

    void HandleMovementInput()
    {
        moveInput = Vector2.zero;

        if (Input.GetKey(KeyCode.W)) moveInput.y += 1;
        if (Input.GetKey(KeyCode.S)) moveInput.y -= 1;
        if (Input.GetKey(KeyCode.A)) moveInput.x -= 1;
        if (Input.GetKey(KeyCode.D)) moveInput.x += 1;

        if (moveInput != Vector2.zero)
            moveInput.Normalize();
    }

    void HandleAiming()
    {
        if (mainCam == null) mainCam = Camera.main;
        if (mainCam == null) return;

        Vector3 mouseWorldPos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 aimDir = ((Vector2)mouseWorldPos - (Vector2)transform.position).normalized;

        if (aimDir.sqrMagnitude > 0.01f)
        {
            float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
            if (visualChild != null)
                visualChild.rotation = Quaternion.Euler(0, 0, angle - 90);
            else
                transform.rotation = Quaternion.Euler(0, 0, angle - 90);
        }
    }

    void HandleSprint()
    {
        bool wantsSprint = Input.GetKey(KeyCode.LeftShift) && moveInput != Vector2.zero && currentStamina > 0f;

        if (wantsSprint)
        {
            isSprinting = true;
            currentStamina -= staminaDrainRate * Time.deltaTime;
            currentStamina = Mathf.Max(0f, currentStamina);
            regenDelayTimer = staminaRegenDelay;

            if (currentStamina <= 0f)
                isSprinting = false;
        }
        else
        {
            isSprinting = false;
            regenDelayTimer -= Time.deltaTime;

            if (regenDelayTimer <= 0f && currentStamina < maxStamina)
            {
                currentStamina += staminaRegenRate * Time.deltaTime;
                currentStamina = Mathf.Min(maxStamina, currentStamina);
            }
        }
    }

    void HandleAttack()
    {
        attackTimer -= Time.deltaTime;

        if (Input.GetMouseButtonDown(0) && attackTimer <= 0f)
        {
            Fire();
            attackTimer = HasFirepowerBoost ? attackCooldown * 0.5f : attackCooldown;
        }
    }

    void Fire()
    {
        if (bulletPrefab == null) return;
        if (mainCam == null) mainCam = Camera.main;
        if (mainCam == null) return;

        Vector3 mouseWorldPos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        Vector2 aimDir = ((Vector2)mouseWorldPos - (Vector2)transform.position).normalized;

        Vector3 spawnPos = transform.position + (Vector3)(aimDir * 0.5f);

        GameObject bulletObj = BulletPool.Instance != null
            ? BulletPool.Instance.GetBullet()
            : Instantiate(bulletPrefab);

        bulletObj.transform.position = spawnPos;
        float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
        bulletObj.transform.rotation = Quaternion.Euler(0, 0, angle - 90);

        Bullet bullet = bulletObj.GetComponent<Bullet>();
        if (bullet != null)
        {
            int dmg = HasFirepowerBoost ? bulletDamage * 2 : bulletDamage;
            bullet.Initialize(aimDir, bulletSpeed, dmg, true);
        }

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(AudioManager.Instance.shootSFX);
    }

    void HandleFirepowerBoost()
    {
        if (HasFirepowerBoost)
        {
            firepowerBoostTimer -= Time.deltaTime;
            if (firepowerBoostTimer <= 0f)
            {
                HasFirepowerBoost = false;
            }
        }
    }

    public void ActivateFirepowerBoost(float duration)
    {
        HasFirepowerBoost = true;
        firepowerBoostTimer = duration;
    }
}
