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
        currentMode = GameMode.Wave;
        UpdateCoinsUI();
        StartWave();
    }

    void Update()
    {
        // debug money
        if (Input.GetKeyDown(KeyCode.Backslash))
        {
            AddMoney(1000);
        }

        // check if wave is over
        if (waveActive)
        {
            CheckWaveEnd();
        }
    }

    void StartWave()
    {
        currentMode = GameMode.Wave;
        waveActive = true;

        if (startWaveButton != null)
            startWaveButton.SetActive(false);

        // calculate enemy amount
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
        Debug.Log("Enemies spawning: " + enemiesThisWave);

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
        int npcCount = Mathf.Clamp(currentWave, 1, 10);

        for (int i = 0; i < npcCount; i++)
        {
            Vector2 spawnPos = new Vector2(
                Random.Range(-20, 40),
                Random.Range(-60, 50)
            );

            Instantiate(npcPrefab, spawnPos, Quaternion.identity);
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
}