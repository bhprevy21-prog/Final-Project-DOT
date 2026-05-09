using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;
using TMPro;
using System.Collections;

public class WaveManager : MonoBehaviour
{
    public enum GameMode
    {
        Wave,
        Build,
        NightWave
    }

    [Header("Mode")]
    public GameMode currentMode;

   [Header("References")]
public GameObject startWaveButton;
public GameObject enemyPrefab;
public GameObject npcPrefab;


    [Header("Spawn Points")]
    public Transform[] spawnPoints;

    [Header("NPC Spawn")]
    public Transform npcSpawnPoint;

    [Header("Systems")]
    public StatueHealth statue;
    public static WaveManager Instance;

    [Header("Stats")]
    public int currentWave = 1;

    [Header("Wave Intro UI")]
    public TextMeshProUGUI waveIntroText;
    public CanvasGroup waveIntroGroup;

    [Header("Night Visuals")]
    public GameObject nightOverlay;
    public float nightAlpha = 0.99f;

    [Header("Cinemachine")]
    public CinemachineVirtualCamera vCam;
    public float normalZoom = 5f;
    public float nightZoom = 3.5f;

    [Header("UI")]
    public StartButtonUI startButtonUI;

    [Header("Coin UI (TEXT ONLY)")]
    public TextMeshProUGUI CoinText;
    public TextMeshProUGUI ShopCoins;

    private int previousEnemyCount = 10;
    private int enemiesThisWave = 10;

    private bool waveActive = false;

    // =========================
    // START
    // =========================

    void Awake()
    {
        Instance = this;
    }

    void Start()
{
    LoadGameState();
    UpdateCoinsUI();
    RestorePlayerPosition();
    UpdateWaveUI();

    if (currentMode == GameMode.Build)
    {
        waveActive = false;
        startButtonUI?.SetBuildMode();
    }
    else
    {
        StartWave();
    }

    UpdateNightVisuals();
    UpdateCameraZoom();
}

    // =========================
    // UPDATE
    // =========================

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backslash))
            AddMoney(1000);

        if (waveActive)
            CheckWaveEnd();

        if (Input.GetKeyDown(KeyCode.Quote))
            ClearSaveData();
    }

    // =========================
    // WAVE START
    // =========================

    void StartWave()
{
    waveActive = true;

    currentMode = (currentWave % 3 == 0)
        ? GameMode.NightWave
        : GameMode.Wave;

    startButtonUI?.SetWaveMode();
Debug.Log("Starting Wave " + currentWave + " | Mode = " + currentMode);
    if (currentWave == 1)
        enemiesThisWave = 10;
    else
        enemiesThisWave = Mathf.RoundToInt(previousEnemyCount * 1.5f);

    if (currentMode == GameMode.NightWave)
        enemiesThisWave = Mathf.RoundToInt(enemiesThisWave * 1.3f);

    previousEnemyCount = enemiesThisWave;

    SpawnEnemies();
    SpawnNPCs();

    UpdateNightVisuals();
    UpdateCameraZoom();

    StartCoroutine(PlayWaveIntro());
}

    // =========================
    // SPAWNING
    // =========================

    void SpawnEnemies()
{
    for (int i = 0; i < enemiesThisWave; i++)
    {
        Transform spawn =
            spawnPoints[Random.Range(0, spawnPoints.Length)];

        Instantiate(
            enemyPrefab,
            spawn.position,
            Quaternion.identity
        );
    }
}
    void SpawnNPCs()
    {
        int npcCount = Mathf.Clamp(currentWave, 1, 10);

        for (int i = 0; i < npcCount; i++)
        {
            Vector2 offset = Random.insideUnitCircle;
            Instantiate(npcPrefab, (Vector2)npcSpawnPoint.position + offset, Quaternion.identity);
        }
    }

    // =========================
    // WAVE END
    // =========================

    void CheckWaveEnd()
    {
        if (GameObject.FindGameObjectsWithTag("Enemy").Length == 0 &&
            GameObject.FindGameObjectsWithTag("NPC").Length == 0)
        {
            waveActive = false;
            EnterBuildMode();
        }
    }

   void EnterBuildMode()
{
    currentMode = GameMode.Build;

    if (statue != null)
        statue.Heal(50);

    AddMoney(enemiesThisWave * 5);

    startButtonUI?.SetBuildMode();

    UpdateNightVisuals();
    UpdateCameraZoom();

    SaveGameState(); // ONLY HERE
}

   public void StartNextWave()
{
    if (currentMode != GameMode.Build)
        return;

    currentWave++;
    StartWave();
}

    // =========================
    // MONEY SYSTEM (FIXED CLEAN)
    // =========================

    public int playerMoney = 0;

    public void AddMoney(int amount)
{
    playerMoney += amount;
    UpdateCoinsUI();
}

    void UpdateCoinsUI()
    {
        if (CoinText != null)
            CoinText.text = playerMoney.ToString();

        if (ShopCoins != null)
            ShopCoins.text = playerMoney.ToString();
    }

    // =========================
    // VISUALS
    // =========================

   void UpdateNightVisuals()
{
    if (nightOverlay == null)
    {
        Debug.Log("NO NIGHT OVERLAY ASSIGNED");
        return;
    }

    nightOverlay.SetActive(true);

    CanvasGroup cg = nightOverlay.GetComponent<CanvasGroup>();
    if (cg == null)
        cg = nightOverlay.AddComponent<CanvasGroup>();

    bool isNight = currentMode == GameMode.NightWave;

    cg.alpha = isNight ? nightAlpha : 0f;
    cg.blocksRaycasts = false;
    cg.interactable = false;

    Debug.Log("Overlay active | isNight = " + isNight + " | alpha = " + cg.alpha);
}

    void UpdateCameraZoom()
    {
        if (vCam == null) return;

        var lens = vCam.m_Lens;
        lens.OrthographicSize = (currentMode == GameMode.NightWave) ? nightZoom : normalZoom;
        vCam.m_Lens = lens;
    }

    // =========================
    // SAVE / LOAD
    // =========================

    public void SaveGameState()
{
    PlayerPrefs.SetInt("SavedWave", currentWave);
    PlayerPrefs.SetInt("SavedMode", (int)GameMode.Build);
    PlayerPrefs.SetInt("SavedMoney", playerMoney);
    PlayerPrefs.Save();
}

    public void LoadGameState()
    {
        currentWave = PlayerPrefs.GetInt("SavedWave", 1);
        currentMode = (GameMode)PlayerPrefs.GetInt("SavedMode", 0);
        playerMoney = PlayerPrefs.GetInt("SavedMoney", 0);
    }

    // =========================
    // UI HELP
    // =========================

    void UpdateWaveUI()
    {
        if (waveIntroText != null)
            waveIntroText.text = "Wave " + currentWave;
    }

    IEnumerator PlayWaveIntro()
    {
        if (waveIntroText == null || waveIntroGroup == null)
            yield break;

        UpdateWaveUI();

        RectTransform rt = waveIntroText.rectTransform;

        Vector2 start = new Vector2(900f, rt.anchoredPosition.y);
        Vector2 center = new Vector2(0f, rt.anchoredPosition.y);

        rt.anchoredPosition = start;
        waveIntroGroup.alpha = 1f;

        float t = 0f;

        while (t < 1.25f)
        {
            t += Time.deltaTime;
            rt.anchoredPosition = Vector2.Lerp(start, center, t / 1.25f);
            yield return null;
        }

        yield return new WaitForSeconds(1f);

        t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime;
            waveIntroGroup.alpha = Mathf.Lerp(1f, 0f, t);
            yield return null;
        }

        waveIntroGroup.alpha = 0f;
    }

    // =========================
    // RESET
    // =========================

    public void ClearSaveData()
{
    PlayerPrefs.DeleteAll();
    PlayerPrefs.Save();

    // clear inventory too
    if (PlayerInventory.Instance != null)
    {
        PlayerInventory.Instance.hotbar = new string[4];
        PlayerInventory.Instance.backpack.Clear();
        PlayerInventory.Instance.selectedSlot = -1;
        PlayerInventory.Instance.selectedItem = "";
    }

    Debug.Log("SAVE CLEARED");

    SceneManager.LoadScene("WaveScene");
}

    void RestorePlayerPosition()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (!player) return;

        float x = PlayerPrefs.GetFloat("PlayerX", player.transform.position.x);
        float y = PlayerPrefs.GetFloat("PlayerY", player.transform.position.y);

        player.transform.position = new Vector3(x, y, player.transform.position.z);
    }
}