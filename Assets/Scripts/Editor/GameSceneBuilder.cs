using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class GameSceneBuilder : EditorWindow
{
    [MenuItem("DungeonGame/Build All Scenes")]
    public static void BuildAllScenes()
    {
        SetupTagsAndLayers();
        SetupPhysics2D();
        BuildMainMenuScene();
        BuildGameLevelScene();
        SetupBuildSettings();
        Debug.Log("All scenes built successfully!");
    }

    [MenuItem("DungeonGame/Setup Tags and Layers Only")]
    public static void SetupTagsAndLayers()
    {
        AddTag("Player");
        AddTag("Enemy");
        AddTag("Wall");
        AddTag("IndestructibleWall");
        AddTag("Prop");
        AddTag("Bullet");

        AddLayer(6, "Player");
        AddLayer(7, "Enemy");
        AddLayer(8, "PlayerBullet");
        AddLayer(9, "EnemyBullet");
        AddLayer(10, "Wall");
        AddLayer(11, "Prop");

        Debug.Log("Tags and layers configured.");
    }

    static void SetupPhysics2D()
    {
        int playerLayer = 6;
        int enemyLayer = 7;
        int playerBulletLayer = 8;
        int enemyBulletLayer = 9;
        int wallLayer = 10;
        int propLayer = 11;

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
        Physics2D.IgnoreLayerCollision(playerLayer, enemyLayer, true);

        Debug.Log("Physics2D collision matrix configured.");
    }

    static void BuildMainMenuScene()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

        Camera.main.orthographic = true;
        Camera.main.orthographicSize = 5f;
        Camera.main.backgroundColor = new Color(0.06f, 0.06f, 0.1f);

        GameObject initObj = new GameObject("MainMenuInit");
        initObj.AddComponent<MainMenuInit>();

        GameObject bgObj = new GameObject("MenuBackground");
        bgObj.AddComponent<MenuBackground>();

        GameObject canvas = CreateCanvas("MenuCanvas");
        MainMenuUI menuUI = canvas.AddComponent<MainMenuUI>();

        // ===== Main Panel =====
        GameObject mainPanel = CreatePanel(canvas.transform, "MainPanel");
        menuUI.mainPanel = mainPanel;

        // Center card background
        GameObject cardBg = new GameObject("CardBackground");
        cardBg.transform.SetParent(mainPanel.transform, false);
        RectTransform cardRT = cardBg.AddComponent<RectTransform>();
        cardRT.anchoredPosition = new Vector2(0, 10);
        cardRT.sizeDelta = new Vector2(420, 520);
        Image cardImg = cardBg.AddComponent<Image>();
        cardImg.color = new Color(0.08f, 0.1f, 0.18f, 0.85f);

        // Decorative top accent line
        GameObject accentLine = new GameObject("AccentLine");
        accentLine.transform.SetParent(cardBg.transform, false);
        RectTransform alRT = accentLine.AddComponent<RectTransform>();
        alRT.anchorMin = new Vector2(0.15f, 1);
        alRT.anchorMax = new Vector2(0.85f, 1);
        alRT.pivot = new Vector2(0.5f, 1);
        alRT.anchoredPosition = Vector2.zero;
        alRT.sizeDelta = new Vector2(0, 3);
        Image alImg = accentLine.AddComponent<Image>();
        alImg.color = new Color(1f, 0.75f, 0.3f, 0.9f);

        // Title
        CreateMenuTitle(cardBg.transform, "DUNGEON OF SIP", 52, new Vector2(0, -40));

        // Subtitle
        GameObject subObj = new GameObject("Subtitle");
        subObj.transform.SetParent(cardBg.transform, false);
        TextMeshProUGUI subTmp = subObj.AddComponent<TextMeshProUGUI>();
        subTmp.text = "A Top-Down Dungeon Adventure";
        subTmp.fontSize = 16;
        subTmp.alignment = TextAlignmentOptions.Center;
        subTmp.color = new Color(0.6f, 0.65f, 0.8f, 0.8f);
        subTmp.fontStyle = FontStyles.Italic;
        RectTransform subRT = subObj.GetComponent<RectTransform>();
        subRT.anchorMin = new Vector2(0.5f, 1);
        subRT.anchorMax = new Vector2(0.5f, 1);
        subRT.pivot = new Vector2(0.5f, 1);
        subRT.anchoredPosition = new Vector2(0, -100);
        subRT.sizeDelta = new Vector2(380, 30);

        // Separator line
        GameObject sepLine = new GameObject("Separator");
        sepLine.transform.SetParent(cardBg.transform, false);
        RectTransform sepRT = sepLine.AddComponent<RectTransform>();
        sepRT.anchorMin = new Vector2(0.5f, 1);
        sepRT.anchorMax = new Vector2(0.5f, 1);
        sepRT.pivot = new Vector2(0.5f, 0.5f);
        sepRT.anchoredPosition = new Vector2(0, -140);
        sepRT.sizeDelta = new Vector2(280, 1);
        Image sepImg = sepLine.AddComponent<Image>();
        sepImg.color = new Color(0.4f, 0.45f, 0.6f, 0.4f);

        // Buttons inside card
        float btnY = -180f;
        float btnSpacing = 58f;
        menuUI.startButton = CreateMenuButton(cardBg.transform, "StartButton", "START GAME",
            new Vector2(0, btnY), new Color(0.2f, 0.55f, 0.35f), true);
        menuUI.settingsButton = CreateMenuButton(cardBg.transform, "SettingsButton", "SETTINGS",
            new Vector2(0, btnY - btnSpacing), new Color(0.2f, 0.35f, 0.55f), false);
        menuUI.leaderboardButton = CreateMenuButton(cardBg.transform, "LeaderboardButton", "LEADERBOARD",
            new Vector2(0, btnY - btnSpacing * 2), new Color(0.2f, 0.35f, 0.55f), false);
        menuUI.quitButton = CreateMenuButton(cardBg.transform, "QuitButton", "QUIT",
            new Vector2(0, btnY - btnSpacing * 3), new Color(0.4f, 0.2f, 0.2f), false);

        // Version text
        GameObject verObj = new GameObject("VersionText");
        verObj.transform.SetParent(mainPanel.transform, false);
        TextMeshProUGUI verTmp = verObj.AddComponent<TextMeshProUGUI>();
        verTmp.text = "v1.0";
        verTmp.fontSize = 14;
        verTmp.alignment = TextAlignmentOptions.Center;
        verTmp.color = new Color(0.4f, 0.4f, 0.5f, 0.5f);
        RectTransform verRT = verObj.GetComponent<RectTransform>();
        verRT.anchorMin = new Vector2(0.5f, 0);
        verRT.anchorMax = new Vector2(0.5f, 0);
        verRT.pivot = new Vector2(0.5f, 0);
        verRT.anchoredPosition = new Vector2(0, 15);
        verRT.sizeDelta = new Vector2(200, 25);

        // ===== Settings Panel =====
        GameObject settingsPanel = CreatePanel(canvas.transform, "SettingsPanel");
        settingsPanel.SetActive(false);
        menuUI.settingsPanel = settingsPanel;

        // Settings card background
        GameObject settingsCard = new GameObject("SettingsCard");
        settingsCard.transform.SetParent(settingsPanel.transform, false);
        RectTransform scRT = settingsCard.AddComponent<RectTransform>();
        scRT.anchoredPosition = new Vector2(0, 10);
        scRT.sizeDelta = new Vector2(420, 380);
        Image scImg = settingsCard.AddComponent<Image>();
        scImg.color = new Color(0.08f, 0.1f, 0.18f, 0.9f);

        SettingsUI settingsUI = settingsPanel.AddComponent<SettingsUI>();

        CreateMenuTitle(settingsCard.transform, "SETTINGS", 38, new Vector2(0, -35));

        // Separator
        GameObject setSep = new GameObject("Separator");
        setSep.transform.SetParent(settingsCard.transform, false);
        RectTransform setSepRT = setSep.AddComponent<RectTransform>();
        setSepRT.anchorMin = new Vector2(0.5f, 1);
        setSepRT.anchorMax = new Vector2(0.5f, 1);
        setSepRT.pivot = new Vector2(0.5f, 0.5f);
        setSepRT.anchoredPosition = new Vector2(0, -85);
        setSepRT.sizeDelta = new Vector2(280, 1);
        Image setSepImg = setSep.AddComponent<Image>();
        setSepImg.color = new Color(0.4f, 0.45f, 0.6f, 0.4f);

        settingsUI.bgmSlider = CreateSliderWithLabel(settingsCard.transform, "BGM Volume", new Vector2(0, -130));
        settingsUI.sfxSlider = CreateSliderWithLabel(settingsCard.transform, "SFX Volume", new Vector2(0, -200));
        settingsUI.backButton = CreateMenuButton(settingsCard.transform, "BackFromSettings", "BACK",
            new Vector2(0, -290), new Color(0.3f, 0.3f, 0.4f), false);

        // ===== Leaderboard Panel =====
        GameObject leaderboardPanel = CreatePanel(canvas.transform, "LeaderboardPanel");
        leaderboardPanel.SetActive(false);
        menuUI.leaderboardPanel = leaderboardPanel;

        // Leaderboard card background
        GameObject lbCard = new GameObject("LeaderboardCard");
        lbCard.transform.SetParent(leaderboardPanel.transform, false);
        RectTransform lbcRT = lbCard.AddComponent<RectTransform>();
        lbcRT.anchoredPosition = new Vector2(0, 10);
        lbcRT.sizeDelta = new Vector2(480, 500);
        Image lbcImg = lbCard.AddComponent<Image>();
        lbcImg.color = new Color(0.08f, 0.1f, 0.18f, 0.9f);

        LeaderboardUI lbUI = leaderboardPanel.AddComponent<LeaderboardUI>();

        CreateMenuTitle(lbCard.transform, "LEADERBOARD", 38, new Vector2(0, -35));

        // Separator
        GameObject lbSep = new GameObject("Separator");
        lbSep.transform.SetParent(lbCard.transform, false);
        RectTransform lbSepRT = lbSep.AddComponent<RectTransform>();
        lbSepRT.anchorMin = new Vector2(0.5f, 1);
        lbSepRT.anchorMax = new Vector2(0.5f, 1);
        lbSepRT.pivot = new Vector2(0.5f, 0.5f);
        lbSepRT.anchoredPosition = new Vector2(0, -85);
        lbSepRT.sizeDelta = new Vector2(380, 1);
        Image lbSepImg = lbSep.AddComponent<Image>();
        lbSepImg.color = new Color(0.4f, 0.45f, 0.6f, 0.4f);

        // Column headers
        GameObject headerObj = new GameObject("ColumnHeaders");
        headerObj.transform.SetParent(lbCard.transform, false);
        TextMeshProUGUI headerTmp = headerObj.AddComponent<TextMeshProUGUI>();
        headerTmp.text = "RANK     NAME          SCORE       TIME";
        headerTmp.fontSize = 14;
        headerTmp.alignment = TextAlignmentOptions.Center;
        headerTmp.color = new Color(0.6f, 0.65f, 0.8f, 0.7f);
        RectTransform headerRT = headerObj.GetComponent<RectTransform>();
        headerRT.anchorMin = new Vector2(0.5f, 1);
        headerRT.anchorMax = new Vector2(0.5f, 1);
        headerRT.pivot = new Vector2(0.5f, 1);
        headerRT.anchoredPosition = new Vector2(0, -95);
        headerRT.sizeDelta = new Vector2(420, 25);

        GameObject scrollArea = new GameObject("ScrollArea");
        scrollArea.transform.SetParent(lbCard.transform, false);
        RectTransform scrollRT = scrollArea.AddComponent<RectTransform>();
        scrollRT.anchorMin = new Vector2(0.5f, 1);
        scrollRT.anchorMax = new Vector2(0.5f, 1);
        scrollRT.pivot = new Vector2(0.5f, 1);
        scrollRT.anchoredPosition = new Vector2(0, -125);
        scrollRT.sizeDelta = new Vector2(420, 280);

        GameObject entriesContainer = new GameObject("EntriesContainer");
        entriesContainer.transform.SetParent(scrollArea.transform, false);
        RectTransform contRT = entriesContainer.AddComponent<RectTransform>();
        contRT.anchorMin = new Vector2(0, 1);
        contRT.anchorMax = new Vector2(1, 1);
        contRT.pivot = new Vector2(0.5f, 1);
        contRT.anchoredPosition = Vector2.zero;
        contRT.sizeDelta = new Vector2(0, 280);
        VerticalLayoutGroup vlg = entriesContainer.AddComponent<VerticalLayoutGroup>();
        vlg.spacing = 4;
        vlg.childAlignment = TextAnchor.UpperCenter;
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false;
        vlg.padding = new RectOffset(10, 10, 5, 5);

        lbUI.entriesContainer = entriesContainer.transform;
        GameObject entryPrefab = CreateLeaderboardEntryPrefab();
        entryPrefab.transform.SetParent(leaderboardPanel.transform, false);
        entryPrefab.SetActive(false);
        lbUI.entryPrefab = entryPrefab;
        lbUI.backButton = CreateMenuButton(lbCard.transform, "BackFromLeaderboard", "BACK",
            new Vector2(0, -445), new Color(0.3f, 0.3f, 0.4f), false);

        string scenePath = "Assets/Scenes/MainMenu.unity";
        EditorSceneManager.SaveScene(scene, scenePath);
        Debug.Log("MainMenu scene built at " + scenePath);
    }

    static void BuildGameLevelScene()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

        Camera.main.orthographic = true;
        Camera.main.orthographicSize = 12f;
        Camera.main.backgroundColor = new Color(0.1f, 0.1f, 0.15f);

        GameObject levelController = new GameObject("LevelController");
        levelController.AddComponent<MapGenerator>();
        levelController.AddComponent<LevelManager>();
        levelController.AddComponent<PropSpawner>();
        levelController.AddComponent<GameLevelInit>();
        levelController.AddComponent<BulletAssigner>();

        GameObject bulletPool = new GameObject("BulletPool");
        bulletPool.AddComponent<BulletPool>();

        GameObject vfx = new GameObject("VFXManager");
        vfx.AddComponent<VFXManager>();

        GameObject canvas = CreateCanvas("GameCanvas");
        UIManager uiMgr = canvas.AddComponent<UIManager>();

        // HUD Panel
        GameObject hudPanel = CreatePanel(canvas.transform, "HUDPanel");
        HUDController hud = hudPanel.AddComponent<HUDController>();
        uiMgr.hudPanel = hudPanel;

        GameObject heartsContainer = new GameObject("HeartsContainer");
        heartsContainer.transform.SetParent(hudPanel.transform, false);
        RectTransform hcRT = heartsContainer.AddComponent<RectTransform>();
        hcRT.anchorMin = new Vector2(0, 1);
        hcRT.anchorMax = new Vector2(0, 1);
        hcRT.pivot = new Vector2(0, 1);
        hcRT.anchoredPosition = new Vector2(10, -10);
        hcRT.sizeDelta = new Vector2(200, 30);
        HorizontalLayoutGroup hlg = heartsContainer.AddComponent<HorizontalLayoutGroup>();
        hlg.spacing = 5;
        hlg.childAlignment = TextAnchor.MiddleLeft;
        hlg.childForceExpandWidth = false;
        hlg.childForceExpandHeight = false;
        hud.heartsContainer = heartsContainer.transform;

        hud.timerText = CreateTMPText(hudPanel.transform, "TimerText", "00:00", 24,
            new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -10));
        hud.scoreText = CreateTMPText(hudPanel.transform, "ScoreText", "Score: 0", 22,
            new Vector2(1, 1), new Vector2(1, 1), new Vector2(1, 1), new Vector2(-10, -10));
        hud.livesText = CreateTMPText(hudPanel.transform, "LivesText", "Lives: 10", 20,
            new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1), new Vector2(10, -45));
        hud.enemyCountText = CreateTMPText(hudPanel.transform, "EnemyCountText", "Enemies: 0/10", 20,
            new Vector2(1, 1), new Vector2(1, 1), new Vector2(1, 1), new Vector2(-10, -45));
        hud.levelText = CreateTMPText(hudPanel.transform, "LevelText", "Level 1", 26,
            new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -45));

        // Stamina Bar
        GameObject staminaContainer = new GameObject("StaminaBarContainer");
        staminaContainer.transform.SetParent(hudPanel.transform, false);
        RectTransform stcRT = staminaContainer.AddComponent<RectTransform>();
        stcRT.anchorMin = new Vector2(0, 0);
        stcRT.anchorMax = new Vector2(0, 0);
        stcRT.pivot = new Vector2(0, 0);
        stcRT.anchoredPosition = new Vector2(15, 15);
        stcRT.sizeDelta = new Vector2(180, 14);

        // Label
        GameObject staminaLabel = new GameObject("StaminaLabel");
        staminaLabel.transform.SetParent(staminaContainer.transform, false);
        TextMeshProUGUI stLabelTmp = staminaLabel.AddComponent<TextMeshProUGUI>();
        stLabelTmp.text = "SPRINT";
        stLabelTmp.fontSize = 11;
        stLabelTmp.alignment = TextAlignmentOptions.Left;
        stLabelTmp.color = new Color(0.7f, 0.75f, 0.85f, 0.8f);
        RectTransform stLblRT = staminaLabel.GetComponent<RectTransform>();
        stLblRT.anchorMin = new Vector2(0, 1);
        stLblRT.anchorMax = new Vector2(1, 1);
        stLblRT.pivot = new Vector2(0, 0);
        stLblRT.anchoredPosition = new Vector2(0, 2);
        stLblRT.sizeDelta = new Vector2(0, 16);

        // Background
        GameObject stBarBg = new GameObject("StaminaBarBg");
        stBarBg.transform.SetParent(staminaContainer.transform, false);
        RectTransform stBgRT = stBarBg.AddComponent<RectTransform>();
        stBgRT.anchorMin = Vector2.zero;
        stBgRT.anchorMax = Vector2.one;
        stBgRT.offsetMin = Vector2.zero;
        stBgRT.offsetMax = Vector2.zero;
        Image stBgImg = stBarBg.AddComponent<Image>();
        stBgImg.color = new Color(0.1f, 0.1f, 0.15f, 0.8f);


        // Fill
        GameObject stBarFill = new GameObject("StaminaBarFill");
        stBarFill.transform.SetParent(staminaContainer.transform, false);
        RectTransform stFillRT = stBarFill.AddComponent<RectTransform>();
        stFillRT.anchorMin = Vector2.zero;
        stFillRT.anchorMax = Vector2.one;
        stFillRT.offsetMin = new Vector2(2, 2);
        stFillRT.offsetMax = new Vector2(-2, -2);
        Image stFillImg = stBarFill.AddComponent<Image>();
        stFillImg.color = new Color(0.2f, 0.9f, 0.4f);
        hud.staminaBarFillRT = stFillRT;
        hud.staminaBarFillImage = stFillImg;

        // Pause Panel
        GameObject pausePanel = CreatePanel(canvas.transform, "PausePanel");
        pausePanel.SetActive(false);
        uiMgr.pausePanel = pausePanel;
        PauseMenuUI pauseUI = pausePanel.AddComponent<PauseMenuUI>();

        Image pauseBg = pausePanel.GetComponent<Image>();
        if (pauseBg != null) pauseBg.color = new Color(0, 0, 0, 0.7f);

        CreateLabel(pausePanel.transform, "PausedText", "PAUSED", 48, new Vector2(0, 80));
        pauseUI.resumeButton = CreateButton(pausePanel.transform, "ResumeButton", "RESUME", new Vector2(0, 0));
        pauseUI.restartButton = CreateButton(pausePanel.transform, "RestartButton", "RESTART", new Vector2(0, -60));
        pauseUI.homeButton = CreateButton(pausePanel.transform, "HomeButton", "HOME", new Vector2(0, -120));

        // Game Over Panel
        GameObject gameOverPanel = CreatePanel(canvas.transform, "GameOverPanel");
        gameOverPanel.SetActive(false);
        uiMgr.gameOverPanel = gameOverPanel;
        GameOverUI goUI = gameOverPanel.AddComponent<GameOverUI>();

        Image goBg = gameOverPanel.GetComponent<Image>();
        if (goBg != null) goBg.color = new Color(0.3f, 0, 0, 0.8f);

        CreateLabel(gameOverPanel.transform, "GameOverText", "GAME OVER", 48, new Vector2(0, 80));
        goUI.scoreText = CreateTMPText(gameOverPanel.transform, "FinalScore", "Score: 0", 30,
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 20));
        goUI.retryButton = CreateButton(gameOverPanel.transform, "RetryButton", "RETRY", new Vector2(0, -40));
        goUI.homeButton = CreateButton(gameOverPanel.transform, "HomeButtonGO", "HOME", new Vector2(0, -100));

        // Victory Panel
        GameObject victoryPanel = CreatePanel(canvas.transform, "VictoryPanel");
        victoryPanel.SetActive(false);
        uiMgr.victoryPanel = victoryPanel;
        VictoryUI vUI = victoryPanel.AddComponent<VictoryUI>();

        Image vicBg = victoryPanel.GetComponent<Image>();
        if (vicBg != null) vicBg.color = new Color(0, 0.2f, 0, 0.8f);

        CreateLabel(victoryPanel.transform, "VictoryText", "VICTORY!", 48, new Vector2(0, 120));
        vUI.scoreText = CreateTMPText(victoryPanel.transform, "VicScore", "Final Score: 0", 30,
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 60));

        GameObject nameInputObj = new GameObject("NameInput");
        nameInputObj.transform.SetParent(victoryPanel.transform, false);
        RectTransform niRT = nameInputObj.AddComponent<RectTransform>();
        niRT.sizeDelta = new Vector2(250, 40);
        niRT.anchoredPosition = new Vector2(0, 0);
        Image niImg = nameInputObj.AddComponent<Image>();
        niImg.color = new Color(0.2f, 0.2f, 0.2f);
        TMP_InputField inputField = nameInputObj.AddComponent<TMP_InputField>();

        GameObject inputTextArea = new GameObject("Text Area");
        inputTextArea.transform.SetParent(nameInputObj.transform, false);
        RectTransform taRT = inputTextArea.AddComponent<RectTransform>();
        taRT.anchorMin = Vector2.zero;
        taRT.anchorMax = Vector2.one;
        taRT.offsetMin = new Vector2(10, 0);
        taRT.offsetMax = new Vector2(-10, 0);
        inputTextArea.AddComponent<RectMask2D>();

        GameObject inputText = new GameObject("Text");
        inputText.transform.SetParent(inputTextArea.transform, false);
        TextMeshProUGUI inputTMP = inputText.AddComponent<TextMeshProUGUI>();
        inputTMP.fontSize = 20;
        inputTMP.color = Color.white;
        RectTransform itRT = inputText.GetComponent<RectTransform>();
        itRT.anchorMin = Vector2.zero;
        itRT.anchorMax = Vector2.one;
        itRT.offsetMin = Vector2.zero;
        itRT.offsetMax = Vector2.zero;

        GameObject placeholder = new GameObject("Placeholder");
        placeholder.transform.SetParent(inputTextArea.transform, false);
        TextMeshProUGUI phTMP = placeholder.AddComponent<TextMeshProUGUI>();
        phTMP.text = "Enter your name...";
        phTMP.fontSize = 20;
        phTMP.color = new Color(0.5f, 0.5f, 0.5f);
        phTMP.fontStyle = FontStyles.Italic;
        RectTransform phRT = placeholder.GetComponent<RectTransform>();
        phRT.anchorMin = Vector2.zero;
        phRT.anchorMax = Vector2.one;
        phRT.offsetMin = Vector2.zero;
        phRT.offsetMax = Vector2.zero;

        inputField.textViewport = taRT;
        inputField.textComponent = inputTMP;
        inputField.placeholder = phTMP;
        vUI.nameInput = inputField;

        vUI.submitButton = CreateButton(victoryPanel.transform, "SubmitButton", "SUBMIT SCORE", new Vector2(0, -60));
        vUI.homeButton = CreateButton(victoryPanel.transform, "HomeButtonVic", "HOME", new Vector2(0, -120));

        string scenePath = "Assets/Scenes/GameLevel.unity";
        EditorSceneManager.SaveScene(scene, scenePath);
        Debug.Log("GameLevel scene built at " + scenePath);
    }

    static void SetupBuildSettings()
    {
        EditorBuildSettingsScene[] scenes = new EditorBuildSettingsScene[]
        {
            new EditorBuildSettingsScene("Assets/Scenes/MainMenu.unity", true),
            new EditorBuildSettingsScene("Assets/Scenes/GameLevel.unity", true)
        };
        EditorBuildSettings.scenes = scenes;
        Debug.Log("Build settings configured.");
    }

    // --- UI Helper Methods ---

    static GameObject CreateCanvas(string name)
    {
        GameObject canvasObj = new GameObject(name);
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10;
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;
        canvasObj.AddComponent<GraphicRaycaster>();

        GameObject eventSystem = new GameObject("EventSystem");
        eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
        eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();

        return canvasObj;
    }

    static GameObject CreatePanel(Transform parent, string name)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(parent, false);
        RectTransform rt = panel.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        Image img = panel.AddComponent<Image>();
        img.color = new Color(0, 0, 0, 0);
        return panel;
    }

    static void CreateTitle(Transform parent, string text, int fontSize)
    {
        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(parent, false);
        TextMeshProUGUI tmp = titleObj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = new Color(1f, 0.85f, 0.4f);
        tmp.fontStyle = FontStyles.Bold;
        RectTransform rt = titleObj.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 1);
        rt.anchorMax = new Vector2(0.5f, 1);
        rt.pivot = new Vector2(0.5f, 1);
        rt.anchoredPosition = new Vector2(0, -80);
        rt.sizeDelta = new Vector2(600, 80);
    }

    static void CreateMenuTitle(Transform parent, string text, int fontSize, Vector2 position)
    {
        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(parent, false);
        TextMeshProUGUI tmp = titleObj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = new Color(1f, 0.82f, 0.35f);
        tmp.fontStyle = FontStyles.Bold;
        RectTransform rt = titleObj.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 1);
        rt.anchorMax = new Vector2(0.5f, 1);
        rt.pivot = new Vector2(0.5f, 1);
        rt.anchoredPosition = position;
        rt.sizeDelta = new Vector2(400, 60);
    }

    static Button CreateMenuButton(Transform parent, string name, string label, Vector2 position, Color baseColor, bool isPrimary)
    {
        GameObject btnObj = new GameObject(name);
        btnObj.transform.SetParent(parent, false);
        RectTransform rt = btnObj.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 1);
        rt.anchorMax = new Vector2(0.5f, 1);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = isPrimary ? new Vector2(300, 50) : new Vector2(280, 44);
        rt.anchoredPosition = position;

        Image img = btnObj.AddComponent<Image>();
        img.color = baseColor;
        Button btn = btnObj.AddComponent<Button>();

        ColorBlock colors = btn.colors;
        colors.normalColor = baseColor;
        colors.highlightedColor = new Color(
            Mathf.Min(baseColor.r + 0.12f, 1f),
            Mathf.Min(baseColor.g + 0.12f, 1f),
            Mathf.Min(baseColor.b + 0.12f, 1f));
        colors.pressedColor = new Color(
            baseColor.r * 0.7f,
            baseColor.g * 0.7f,
            baseColor.b * 0.7f);
        colors.selectedColor = colors.highlightedColor;
        colors.fadeDuration = 0.1f;
        btn.colors = colors;

        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(btnObj.transform, false);
        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = label;
        tmp.fontSize = isPrimary ? 24 : 20;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
        if (isPrimary) tmp.fontStyle = FontStyles.Bold;
        RectTransform textRT = textObj.GetComponent<RectTransform>();
        textRT.anchorMin = Vector2.zero;
        textRT.anchorMax = Vector2.one;
        textRT.offsetMin = Vector2.zero;
        textRT.offsetMax = Vector2.zero;

        return btn;
    }

    static void CreateLabel(Transform parent, string name, string text, int fontSize, Vector2 position)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        TextMeshProUGUI tmp = obj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
        tmp.fontStyle = FontStyles.Bold;
        RectTransform rt = obj.GetComponent<RectTransform>();
        rt.anchoredPosition = position;
        rt.sizeDelta = new Vector2(400, 60);
    }

    static Button CreateButton(Transform parent, string name, string label, Vector2 position)
    {
        GameObject btnObj = new GameObject(name);
        btnObj.transform.SetParent(parent, false);
        RectTransform rt = btnObj.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(250, 45);
        rt.anchoredPosition = position;

        Image img = btnObj.AddComponent<Image>();
        img.color = new Color(0.2f, 0.3f, 0.5f);
        Button btn = btnObj.AddComponent<Button>();

        ColorBlock colors = btn.colors;
        colors.normalColor = new Color(0.2f, 0.3f, 0.5f);
        colors.highlightedColor = new Color(0.3f, 0.4f, 0.6f);
        colors.pressedColor = new Color(0.15f, 0.2f, 0.35f);
        btn.colors = colors;

        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(btnObj.transform, false);
        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = label;
        tmp.fontSize = 22;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
        RectTransform textRT = textObj.GetComponent<RectTransform>();
        textRT.anchorMin = Vector2.zero;
        textRT.anchorMax = Vector2.one;
        textRT.offsetMin = Vector2.zero;
        textRT.offsetMax = Vector2.zero;

        return btn;
    }

    static TextMeshProUGUI CreateTMPText(Transform parent, string name, string defaultText, int fontSize,
        Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 position)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        TextMeshProUGUI tmp = obj.AddComponent<TextMeshProUGUI>();
        tmp.text = defaultText;
        tmp.fontSize = fontSize;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
        RectTransform rt = obj.GetComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.pivot = pivot;
        rt.anchoredPosition = position;
        rt.sizeDelta = new Vector2(250, 40);
        return tmp;
    }

    static Slider CreateSliderWithLabel(Transform parent, string label, Vector2 position)
    {
        GameObject container = new GameObject(label.Replace(" ", "") + "Container");
        container.transform.SetParent(parent, false);
        RectTransform crt = container.AddComponent<RectTransform>();
        crt.sizeDelta = new Vector2(350, 40);
        crt.anchoredPosition = position;

        GameObject labelObj = new GameObject("Label");
        labelObj.transform.SetParent(container.transform, false);
        TextMeshProUGUI tmp = labelObj.AddComponent<TextMeshProUGUI>();
        tmp.text = label;
        tmp.fontSize = 20;
        tmp.color = Color.white;
        tmp.alignment = TextAlignmentOptions.Left;
        RectTransform lrt = labelObj.GetComponent<RectTransform>();
        lrt.anchorMin = new Vector2(0, 0);
        lrt.anchorMax = new Vector2(0.35f, 1);
        lrt.offsetMin = Vector2.zero;
        lrt.offsetMax = Vector2.zero;

        GameObject sliderObj = new GameObject("Slider");
        sliderObj.transform.SetParent(container.transform, false);
        RectTransform srt = sliderObj.AddComponent<RectTransform>();
        srt.anchorMin = new Vector2(0.4f, 0.3f);
        srt.anchorMax = new Vector2(1f, 0.7f);
        srt.offsetMin = Vector2.zero;
        srt.offsetMax = Vector2.zero;

        // Background
        GameObject bg = new GameObject("Background");
        bg.transform.SetParent(sliderObj.transform, false);
        Image bgImg = bg.AddComponent<Image>();
        bgImg.color = new Color(0.12f, 0.14f, 0.2f);
        RectTransform bgRT = bg.GetComponent<RectTransform>();
        bgRT.anchorMin = Vector2.zero;
        bgRT.anchorMax = Vector2.one;
        bgRT.offsetMin = Vector2.zero;
        bgRT.offsetMax = Vector2.zero;

        // Fill Area
        GameObject fillArea = new GameObject("Fill Area");
        fillArea.transform.SetParent(sliderObj.transform, false);
        RectTransform faRT = fillArea.AddComponent<RectTransform>();
        faRT.anchorMin = new Vector2(0, 0.25f);
        faRT.anchorMax = new Vector2(1, 0.75f);
        faRT.offsetMin = new Vector2(5, 0);
        faRT.offsetMax = new Vector2(-5, 0);

        GameObject fill = new GameObject("Fill");
        fill.transform.SetParent(fillArea.transform, false);
        Image fillImg = fill.AddComponent<Image>();
        fillImg.color = new Color(0.25f, 0.55f, 0.4f);
        RectTransform fRT = fill.GetComponent<RectTransform>();
        fRT.anchorMin = Vector2.zero;
        fRT.anchorMax = Vector2.one;
        fRT.offsetMin = Vector2.zero;
        fRT.offsetMax = Vector2.zero;

        // Handle
        GameObject handleArea = new GameObject("Handle Slide Area");
        handleArea.transform.SetParent(sliderObj.transform, false);
        RectTransform haRT = handleArea.AddComponent<RectTransform>();
        haRT.anchorMin = Vector2.zero;
        haRT.anchorMax = Vector2.one;
        haRT.offsetMin = new Vector2(10, 0);
        haRT.offsetMax = new Vector2(-10, 0);

        GameObject handle = new GameObject("Handle");
        handle.transform.SetParent(handleArea.transform, false);
        Image handleImg = handle.AddComponent<Image>();
        handleImg.color = Color.white;
        RectTransform hRT = handle.GetComponent<RectTransform>();
        hRT.sizeDelta = new Vector2(15, 0);

        Slider slider = sliderObj.AddComponent<Slider>();
        slider.fillRect = fRT;
        slider.handleRect = hRT;
        slider.minValue = 0;
        slider.maxValue = 1;
        slider.value = 0.5f;
        slider.targetGraphic = handleImg;

        return slider;
    }

    static GameObject CreateLeaderboardEntryPrefab()
    {
        GameObject obj = new GameObject("LeaderboardEntry");

        Image bgImg = obj.AddComponent<Image>();
        bgImg.color = new Color(0.12f, 0.14f, 0.22f, 0.6f);

        RectTransform rt = obj.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(400, 32);

        LayoutElement le = obj.AddComponent<LayoutElement>();
        le.preferredHeight = 32;
        le.preferredWidth = 400;

        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(obj.transform, false);
        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = "#1  Player  -  100 pts  -  02:30";
        tmp.fontSize = 17;
        tmp.color = new Color(0.85f, 0.88f, 0.95f);
        tmp.alignment = TextAlignmentOptions.Center;
        RectTransform textRT = textObj.GetComponent<RectTransform>();
        textRT.anchorMin = Vector2.zero;
        textRT.anchorMax = Vector2.one;
        textRT.offsetMin = new Vector2(10, 0);
        textRT.offsetMax = new Vector2(-10, 0);

        return obj;
    }

    // --- Tag/Layer Helpers ---

    static void AddTag(string tag)
    {
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty tags = tagManager.FindProperty("tags");

        for (int i = 0; i < tags.arraySize; i++)
        {
            if (tags.GetArrayElementAtIndex(i).stringValue == tag)
                return;
        }

        tags.InsertArrayElementAtIndex(tags.arraySize);
        tags.GetArrayElementAtIndex(tags.arraySize - 1).stringValue = tag;
        tagManager.ApplyModifiedProperties();
    }

    static void AddLayer(int index, string name)
    {
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty layers = tagManager.FindProperty("layers");

        if (index < layers.arraySize)
        {
            SerializedProperty layer = layers.GetArrayElementAtIndex(index);
            if (string.IsNullOrEmpty(layer.stringValue) || layer.stringValue == name)
            {
                layer.stringValue = name;
                tagManager.ApplyModifiedProperties();
            }
        }
    }
}
