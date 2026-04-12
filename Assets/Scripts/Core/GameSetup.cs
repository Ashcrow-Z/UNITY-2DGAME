using UnityEngine;

public class GameSetup : MonoBehaviour
{
    public static GameSetup Instance { get; private set; }

    [Header("Generated Prefab References")]
    public GameObject playerPrefab;
    public GameObject enemyNormalPrefab;
    public GameObject enemyReinforcedPrefab;
    public GameObject bulletPrefab;
    public GameObject destructibleWallPrefab;
    public GameObject indestructibleWallPrefab;
    public GameObject firepowerPropPrefab;
    public GameObject healthPackPropPrefab;
    public GameObject heartIconPrefab;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        SetupPhysicsLayers();

        if (playerPrefab == null)
            CreateAllPrefabs();
    }

    void SetupPhysicsLayers()
    {
        int playerLayer = 6;
        int enemyLayer = 7;
        int playerBulletLayer = 8;
        int enemyBulletLayer = 9;
        int wallLayer = 10;
        int propLayer = 11;

        Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, true);
        Physics2D.IgnoreLayerCollision(playerBulletLayer, playerLayer, true);
        Physics2D.IgnoreLayerCollision(playerBulletLayer, playerBulletLayer, true);
        Physics2D.IgnoreLayerCollision(enemyBulletLayer, enemyLayer, true);
        Physics2D.IgnoreLayerCollision(enemyBulletLayer, enemyBulletLayer, true);
        Physics2D.IgnoreLayerCollision(playerBulletLayer, enemyBulletLayer, true);
        Physics2D.IgnoreLayerCollision(propLayer, enemyLayer, true);
        Physics2D.IgnoreLayerCollision(propLayer, wallLayer, true);
        Physics2D.IgnoreLayerCollision(propLayer, playerBulletLayer, true);
        Physics2D.IgnoreLayerCollision(propLayer, enemyBulletLayer, true);
        Physics2D.IgnoreLayerCollision(propLayer, propLayer, true);
        Physics2D.IgnoreLayerCollision(enemyLayer, enemyLayer, true);
    }

    void CreateAllPrefabs()
    {
        bulletPrefab = CreateBulletPrefab();
        playerPrefab = CreatePlayerPrefab();
        enemyNormalPrefab = CreateEnemyNormalPrefab();
        enemyReinforcedPrefab = CreateEnemyReinforcedPrefab();
        destructibleWallPrefab = CreateWallPrefab(true);
        indestructibleWallPrefab = CreateWallPrefab(false);
        firepowerPropPrefab = CreatePropPrefab("FirepowerProp", new Color(1f, 0.5f, 0f));
        healthPackPropPrefab = CreatePropPrefab("HealthPack", Color.green);
        heartIconPrefab = CreateHeartIconPrefab();

        ParentPrefab(bulletPrefab);
        ParentPrefab(playerPrefab);
        ParentPrefab(enemyNormalPrefab);
        ParentPrefab(enemyReinforcedPrefab);
        ParentPrefab(destructibleWallPrefab);
        ParentPrefab(indestructibleWallPrefab);
        ParentPrefab(firepowerPropPrefab);
        ParentPrefab(healthPackPropPrefab);
        ParentPrefab(heartIconPrefab);
    }

    void ParentPrefab(GameObject prefab)
    {
        if (prefab != null)
        {
            prefab.transform.SetParent(transform);
            prefab.SetActive(false);
        }
    }

    void SafeSetTag(GameObject obj, string tag)
    {
        if (tag == "Untagged" || tag == "Respawn" || tag == "Finish" ||
            tag == "EditorOnly" || tag == "MainCamera" || tag == "Player" ||
            tag == "GameController")
        {
            obj.tag = tag;
            return;
        }
        try
        {
            obj.tag = tag;
        }
        catch (UnityException)
        {
            // Tag not defined yet - will be set after running DungeonGame > Build All Scenes
        }
    }

    void SafeSetLayer(GameObject obj, string layerName)
    {
        int layer = LayerMask.NameToLayer(layerName);
        if (layer >= 0) obj.layer = layer;
    }

    GameObject CreatePlayerPrefab()
    {
        GameObject obj = new GameObject("Player");
        SafeSetTag(obj, "Player");
        SafeSetLayer(obj, "Player");

        SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteGenerator.CreateTriangleSprite(32, new Color(0.2f, 0.6f, 1f));
        sr.sortingOrder = 5;

        obj.transform.localScale = new Vector3(0.8f, 0.8f, 1f);

        BoxCollider2D col = obj.AddComponent<BoxCollider2D>();
        col.size = new Vector2(0.8f, 0.8f);

        Rigidbody2D rb = obj.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        obj.AddComponent<PlayerController>();
        obj.AddComponent<PlayerHealth>();

        obj.SetActive(false);
        return obj;
    }

    GameObject CreateEnemyNormalPrefab()
    {
        GameObject obj = new GameObject("EnemyNormal");
        SafeSetTag(obj, "Enemy");
        SafeSetLayer(obj, "Enemy");

        SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteGenerator.CreateTriangleSprite(32, new Color(1f, 0.3f, 0.3f));
        sr.sortingOrder = 4;

        obj.transform.localScale = new Vector3(0.8f, 0.8f, 1f);

        BoxCollider2D col = obj.AddComponent<BoxCollider2D>();
        col.size = new Vector2(0.8f, 0.8f);

        Rigidbody2D rb = obj.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        obj.AddComponent<EnemyNormal>();
        obj.AddComponent<EnemyAI>();

        obj.SetActive(false);
        return obj;
    }

    GameObject CreateEnemyReinforcedPrefab()
    {
        GameObject obj = new GameObject("EnemyReinforced");
        SafeSetTag(obj, "Enemy");
        SafeSetLayer(obj, "Enemy");

        SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteGenerator.CreateSquareSprite(32, new Color(0.6f, 0.1f, 0.1f));
        sr.sortingOrder = 4;

        obj.transform.localScale = new Vector3(0.8f, 0.8f, 1f);

        BoxCollider2D col = obj.AddComponent<BoxCollider2D>();
        col.size = new Vector2(0.9f, 0.9f);

        Rigidbody2D rb = obj.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        obj.AddComponent<EnemyReinforced>();
        obj.AddComponent<EnemyAI>();

        obj.SetActive(false);
        return obj;
    }

    GameObject CreateBulletPrefab()
    {
        GameObject obj = new GameObject("Bullet");
        SafeSetLayer(obj, "PlayerBullet");

        SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteGenerator.CreateCircleSprite(16, Color.yellow);
        sr.sortingOrder = 6;

        obj.transform.localScale = new Vector3(0.3f, 0.3f, 1f);

        CircleCollider2D col = obj.AddComponent<CircleCollider2D>();
        col.radius = 0.4f;
        col.isTrigger = true;

        Rigidbody2D rb = obj.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        obj.AddComponent<Bullet>();

        obj.SetActive(false);
        return obj;
    }

    GameObject CreateWallPrefab(bool destructible)
    {
        string name = destructible ? "DestructibleWall" : "IndestructibleWall";
        GameObject obj = new GameObject(name);
        SafeSetTag(obj, destructible ? "Wall" : "IndestructibleWall");
        SafeSetLayer(obj, "Wall");

        SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
        Color wallColor = destructible
            ? new Color(0.6f, 0.4f, 0.2f)
            : new Color(0.3f, 0.3f, 0.3f);
        sr.sprite = SpriteGenerator.CreateSquareSprite(32, wallColor);
        sr.sortingOrder = 1;

        BoxCollider2D col = obj.AddComponent<BoxCollider2D>();

        if (destructible)
        {
            Obstacle obstacle = obj.AddComponent<Obstacle>();
            obstacle.isDestructible = true;
            obstacle.health = 3;
        }
        else
        {
            Obstacle obstacle = obj.AddComponent<Obstacle>();
            obstacle.isDestructible = false;
        }

        obj.SetActive(false);
        return obj;
    }

    GameObject CreatePropPrefab(string propName, Color color)
    {
        GameObject obj = new GameObject(propName);
        SafeSetTag(obj, "Prop");
        SafeSetLayer(obj, "Prop");

        SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteGenerator.CreateDiamondSprite(24, color);
        sr.sortingOrder = 3;

        obj.transform.localScale = new Vector3(0.6f, 0.6f, 1f);

        CircleCollider2D col = obj.AddComponent<CircleCollider2D>();
        col.radius = 0.4f;
        col.isTrigger = true;

        if (propName.Contains("Firepower"))
            obj.AddComponent<PropFirepower>();
        else
            obj.AddComponent<PropHealthPack>();

        obj.SetActive(false);
        return obj;
    }

    GameObject CreateHeartIconPrefab()
    {
        GameObject obj = new GameObject("HeartIcon");

        UnityEngine.UI.Image img = obj.AddComponent<UnityEngine.UI.Image>();
        img.color = Color.red;

        RectTransform rt = obj.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(25, 25);

        obj.SetActive(false);
        return obj;
    }
}
