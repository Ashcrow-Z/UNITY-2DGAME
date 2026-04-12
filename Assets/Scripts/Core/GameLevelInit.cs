using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameLevelInit : MonoBehaviour
{
    void Awake()
    {
        SetupCamera();
        SetupPrefabReferences();
        SetupUI();
    }

    void Start()
    {
        if (AudioManager.Instance != null && GameManager.Instance != null)
            AudioManager.Instance.PlayLevelMusic(GameManager.Instance.CurrentLevel);
    }

    void SetupCamera()
    {
        Camera cam = Camera.main;
        if (cam != null)
        {
            cam.orthographic = true;
            cam.orthographicSize = 12f;
            cam.transform.position = new Vector3(0, 0, -10);
            cam.backgroundColor = new Color(0.1f, 0.1f, 0.15f);
        }
    }

    void SetupPrefabReferences()
    {
        GameSetup setup = GameSetup.Instance;
        if (setup == null) return;

        LevelManager lm = GetComponent<LevelManager>();
        if (lm != null)
        {
            lm.playerPrefab = setup.playerPrefab;
            lm.enemyNormalPrefab = setup.enemyNormalPrefab;
            lm.enemyReinforcedPrefab = setup.enemyReinforcedPrefab;
            lm.destructibleWallPrefab = setup.destructibleWallPrefab;
            lm.indestructibleWallPrefab = setup.indestructibleWallPrefab;
        }

        MapGenerator mg = GetComponent<MapGenerator>();
        if (mg != null)
        {
            mg.destructibleWallPrefab = setup.destructibleWallPrefab;
            mg.indestructibleWallPrefab = setup.indestructibleWallPrefab;
        }

        PropSpawner ps = GetComponent<PropSpawner>();
        if (ps != null)
        {
            ps.firepowerPropPrefab = setup.firepowerPropPrefab;
            ps.healthPackPropPrefab = setup.healthPackPropPrefab;
        }

        BulletPool bp = FindObjectOfType<BulletPool>();
        if (bp != null)
        {
            bp.bulletPrefab = setup.bulletPrefab;
        }
    }

    void SetupUI()
    {
        GameSetup setup = GameSetup.Instance;
        if (setup == null) return;

        HUDController hud = FindObjectOfType<HUDController>();
        if (hud != null)
        {
            hud.heartPrefab = setup.heartIconPrefab;
        }
    }
}
