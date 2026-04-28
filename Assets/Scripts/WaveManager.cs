using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public enum GameMode
    {
        Wave,
        Build
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

    private int previousEnemyCount = 10;
    private int enemiesThisWave = 10;

    private bool waveActive = false;

    void Start()
    {
        LoadGameState();
        UpdateCoinsUI();
        RestorePlayerPosition();

        if (currentMode == GameMode.Wave)
        {
            StartWave();
        }
        else
        {
            currentMode = GameMode.Build;

            if (startWaveButton != null)
                startWaveButton.SetActive(true);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backslash))
        {
            AddMoney(1000);
        }
         if (Input.GetKeyDown(KeyCode.Z))
    {
        PlayerInventory.Instance.AddItem("Turret");
        Debug.Log("Gave Turret");
    }

        if (waveActive)
        {
            CheckWaveEnd();
        }
        if (Input.GetKeyDown(KeyCode.Quote))
{
    ClearSaveData();
}
    }

   public void ClearSaveData()
{
    PlayerPrefs.DeleteKey("SavedWave");
    PlayerPrefs.DeleteKey("SavedMode");
    PlayerPrefs.DeleteKey("SavedMoney");
    PlayerPrefs.DeleteKey("PlayerX");
    PlayerPrefs.DeleteKey("PlayerY");

    PlayerPrefs.Save();

    Debug.Log("🧹 Save data cleared!");

    // optional: restart scene immediately
    UnityEngine.SceneManagement.SceneManager.LoadScene("WaveScene");
}
    void StartWave()
    {
        currentMode = GameMode.Wave;
        waveActive = true;

        if (startWaveButton != null)
            startWaveButton.SetActive(false);

        if (currentWave == 1)
        {
            enemiesThisWave = 10;
        }
        else
        {
            enemiesThisWave =
                Mathf.Min(
                    Mathf.RoundToInt(previousEnemyCount * 1.5f) + Random.Range(1, 4),
                    999
                );
        }

        previousEnemyCount = enemiesThisWave;

        Debug.Log("Wave " + currentWave);
        Debug.Log("Enemies: " + enemiesThisWave);

        SpawnEnemies();
        SpawnNPCs();
    }

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
            Vector2 offset = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));

            Instantiate(
                npcPrefab,
                (Vector2)npcSpawnPoint.position + offset,
                Quaternion.identity
            );
        }
    }

    void CheckWaveEnd()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject[] npcs = GameObject.FindGameObjectsWithTag("NPC");

        if (enemies.Length == 0 && npcs.Length == 0)
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

        Debug.Log("Wave cleared!");
    }

    public void StartNextWave()
    {
        if (currentMode != GameMode.Build)
            return;

        currentWave++;
        StartWave();
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
    // SAVE SYSTEM
    // =========================

    public void SaveGameState()
    {
        PlayerPrefs.SetInt("SavedWave", currentWave);
        PlayerPrefs.SetInt("SavedMode", (int)currentMode);
        PlayerPrefs.SetInt("SavedMoney", playerMoney);

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            PlayerPrefs.SetFloat("PlayerX", player.transform.position.x);
            PlayerPrefs.SetFloat("PlayerY", player.transform.position.y);
        }

        PlayerPrefs.Save();
    }

    public void LoadGameState()
    {
        currentWave = PlayerPrefs.GetInt("SavedWave", 1);
        currentMode = (GameMode)PlayerPrefs.GetInt("SavedMode", 0);
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
    
}