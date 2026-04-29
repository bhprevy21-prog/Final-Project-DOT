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
    public CoinUI coinUI;
    public StatueHealth statue;

    [Header("Stats")]
    public int playerMoney = 0;
    public int currentWave = 1;

    [Header("Wave Intro UI")]
    public TextMeshProUGUI waveIntroText;
    public CanvasGroup waveIntroGroup;

    public float introSpeed = 1200f;
    public float fadeSpeed = 2.5f;

    [Header("Night Visuals")]
    public GameObject nightOverlay;
    public float nightAlpha = 0.65f;

    [Header("Cinemachine")]
    public CinemachineVirtualCamera vCam;
    public float normalZoom = 5f;
    public float nightZoom = 3.5f;

    private int previousEnemyCount = 10;
    private int enemiesThisWave = 10;

    private bool waveActive = false;

    // =========================
    // START
    // =========================

    void Start()
{
    LoadGameState();
    UpdateCoinsUI();
    RestorePlayerPosition();
    UpdateWaveUI();

    Debug.Log("Loaded Wave: " + currentWave);
    Debug.Log("Loaded Mode: " + currentMode);

    UpdateNightVisuals();
    UpdateCameraZoom();

    // Resume correct state
    if (currentMode == GameMode.Build)
    {
        waveActive = false;

        if (startWaveButton != null)
            startWaveButton.SetActive(true);
    }
    else
    {
        StartWave();
    }
}

    IEnumerator DelayedIntro()
    {
        yield return null;
        StartCoroutine(PlayWaveIntro());
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
    // DEBUG
    // =========================

    void LogWaveState(string context)
    {
        Debug.Log($"[WaveManager] {context} | Wave: {currentWave} | Mode: {currentMode}");
    }

    // =========================
    // WAVE UI
    // =========================

    void UpdateWaveUI()
    {
        if (waveIntroText != null)
            waveIntroText.text = "Wave " + currentWave;
    }

    // =========================
    // INTRO ANIMATION
    // =========================

    IEnumerator PlayWaveIntro()
{
    if (waveIntroText == null || waveIntroGroup == null)
        yield break;

    UpdateWaveUI();

    waveIntroText.color =
        (currentMode == GameMode.NightWave)
        ? new Color(0.7f, 0.2f, 1f)
        : Color.red;

    RectTransform rt = waveIntroText.rectTransform;

    Vector2 startPos = new Vector2(900f, rt.anchoredPosition.y);
    Vector2 centerPos = new Vector2(0f, rt.anchoredPosition.y);

    rt.anchoredPosition = startPos;
    waveIntroGroup.alpha = 1f;

    float moveTime = 1.25f;
    float holdTime = 1f;
    float fadeTime = 1f;

    // slide in
    float t = 0f;
    while (t < moveTime)
    {
        t += Time.deltaTime;
        rt.anchoredPosition = Vector2.Lerp(startPos, centerPos, t / moveTime);
        yield return null;
    }

    rt.anchoredPosition = centerPos;

    // hold
    yield return new WaitForSeconds(holdTime);

    // fade out
    t = 0f;
    while (t < fadeTime)
    {
        t += Time.deltaTime;
        waveIntroGroup.alpha = Mathf.Lerp(1f, 0f, t / fadeTime);
        yield return null;
    }

    waveIntroGroup.alpha = 0f;
}
    // =========================

    void StartWave()
    {
        waveActive = true;

        if (startWaveButton != null)
            startWaveButton.SetActive(false);

        // 🌙 Night wave every 3rd wave
        if (currentWave % 3 == 0)
        {
            currentMode = GameMode.NightWave;
        }
        else
        {
            currentMode = GameMode.Wave;
        }

        LogWaveState("StartWave");

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
            Transform spawn = spawnPoints[Random.Range(0, spawnPoints.Length)];
            Instantiate(enemyPrefab, spawn.position, Quaternion.identity);
        }
    }

    void SpawnNPCs()
    {
        int npcCount = GetNPCCountForWave(currentWave);

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

        if (startWaveButton != null)
            startWaveButton.SetActive(true);

        UpdateNightVisuals();
        UpdateCameraZoom();

        LogWaveState("EnterBuildMode");
    }

    public void StartNextWave()
    {
        if (currentMode != GameMode.Build)
            return;

        currentWave++;
        StartWave();
    }

    // =========================
    // VISUALS
    // =========================

    void UpdateNightVisuals()
    {
        if (nightOverlay == null)
            return;

        CanvasGroup cg = nightOverlay.GetComponent<CanvasGroup>();

        if (cg == null)
            cg = nightOverlay.AddComponent<CanvasGroup>();

        cg.alpha = (currentMode == GameMode.NightWave) ? nightAlpha : 0f;
    }

    void UpdateCameraZoom()
    {
        if (vCam == null)
            return;

        var lens = vCam.m_Lens;

        lens.OrthographicSize =
            (currentMode == GameMode.NightWave) ? nightZoom : normalZoom;

        vCam.m_Lens = lens;
    }

    // =========================
    // MONEY
    // =========================

    public void AddMoney(int amount)
    {
        playerMoney += amount;
        UpdateCoinsUI();
    }

    void UpdateCoinsUI()
    {
        if (coinUI != null)
            coinUI.SetCoins(playerMoney);
    }

    // =========================
    // SAVE / LOAD
    // =========================

    public void SaveGameState()
    {
        PlayerPrefs.SetInt("SavedWave", currentWave);
        PlayerPrefs.SetInt("SavedMode", (int)currentMode);
        PlayerPrefs.SetInt("SavedMoney", playerMoney);

        PlayerPrefs.Save();
    }

    public void LoadGameState()
{
    currentWave = PlayerPrefs.GetInt("SavedWave", 1);
    currentMode = (GameMode)PlayerPrefs.GetInt("SavedMode", (int)GameMode.Wave);
    playerMoney = PlayerPrefs.GetInt("SavedMoney", 0);
}

    void RestorePlayerPosition()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
            return;

        float x = PlayerPrefs.GetFloat("PlayerX", player.transform.position.x);
        float y = PlayerPrefs.GetFloat("PlayerY", player.transform.position.y);

        player.transform.position = new Vector3(x, y, player.transform.position.z);
    }

    // =========================
    // NPC COUNT
    // =========================

    int GetNPCCountForWave(int wave)
    {
        return Mathf.Clamp(wave, 1, 10);
    }

    // =========================
    // RESET
    // =========================

    public void ClearSaveData()
{
    PlayerPrefs.DeleteAll();
    PlayerPrefs.Save();

    Debug.Log("SAVE CLEARED");

    SceneManager.LoadScene("WaveScene");
}
}