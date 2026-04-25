using UnityEngine;
using System.Collections;

public class WaveManager : MonoBehaviour
{
    public enum GameMode
    {
        Wave,
        Build
    }

    public GameObject startWaveButton;
    public GameMode currentMode;

    [Header("Prefabs")]
    public GameObject enemyPrefab;
    public GameObject npcPrefab;

    [Header("Spawn Points")]
    public Transform[] spawnPoints;

    [Header("UI / Systems")]
    public CoinUI coinUI;
    public StatueHealth statue;

    public int playerMoney = 0;

    private int currentWave = 1;

    private int enemiesAlive = 0;
    private int enemiesThisWave = 0;

    private int npcsAlive = 0;
    private int npcsThisWave = 0;

    private int positiveReviews = 0;
    private int negativeReviews = 0;

    private int spawnIndex = 0;

    private bool spawningFinished = false;

    private bool isNightWave = false;
    public bool statueTriggeredThisWave = false;

    // =========================
    // WAVE SECTION SYSTEM
    // =========================

    [System.Serializable]
    public class WaveSection
    {
        public GameObject enemyPrefab;
        public int count = 3;
        public float spawnDelay = 0.5f;
        public float sectionDelay = 2f;
    }

    [System.Serializable]
    public class WaveData
    {
        public WaveSection[] sections;
    }

    public WaveData[] waveData;

    // =========================
    // START
    // =========================

    void Start()
    {
        currentMode = GameMode.Wave;
        UpdateCoinsUI();
        StartWave();
    }

    // =========================
    // WAVE START
    // =========================

    void StartWave()
    {
        statueTriggeredThisWave = false;
        currentMode = GameMode.Wave;

        if (startWaveButton != null)
            startWaveButton.SetActive(false);

        positiveReviews = 0;
        negativeReviews = 0;

        enemiesAlive = 0;
        enemiesThisWave = 0;

        npcsAlive = GetNPCCountForWave(currentWave);
        npcsThisWave = npcsAlive;

        spawningFinished = false;

        isNightWave = (currentWave % 3 == 0);

        Debug.Log("WAVE " + currentWave + " STARTING");
        Debug.Log(isNightWave ? "🌙 NIGHT WAVE" : "☀ NORMAL WAVE");

        StartCoroutine(SpawnWaveSections());
    }

    // =========================
    // PvZ SECTION SPAWNING
    // =========================

    IEnumerator SpawnWaveSections()
{
    int totalEnemies = GetEnemyCountForWave(currentWave);
    int spawned = 0;

    // =========================
    // WAVES 1–3 (SCRIPTED)
    // =========================
    if (currentWave <= 3 && currentWave - 1 < waveData.Length)
    {
        WaveData wave = waveData[currentWave - 1];

        foreach (WaveSection section in wave.sections)
        {
            int sectionCount = Mathf.Min(section.count, totalEnemies - spawned);

            for (int i = 0; i < sectionCount; i++)
            {
                SpawnEnemy(section.enemyPrefab);

                enemiesAlive++;
                enemiesThisWave++;
                spawned++;

                yield return new WaitForSeconds(section.spawnDelay);
            }

            yield return new WaitForSeconds(section.sectionDelay);
        }
    }
    // =========================
    // WAVES 4+ (ENDLESS PROCEDURAL)
    // =========================
    else
    {
        float spawnDelay = Mathf.Max(0.2f, 1.0f - (currentWave * 0.02f));

        while (spawned < totalEnemies)
        {
            SpawnEnemy(enemyPrefab);

            enemiesAlive++;
            enemiesThisWave++;
            spawned++;

            yield return new WaitForSeconds(spawnDelay);
        }
    }

    // =========================
    // NPC SPAWN
    // =========================
    for (int i = 0; i < npcsThisWave; i++)
    {
        SpawnNPC();
    }

    spawningFinished = true;
}

    void SpawnEnemy(GameObject prefab)
    {
        Transform spawn = spawnPoints[spawnIndex];
        Instantiate(prefab, spawn.position, Quaternion.identity);

        spawnIndex++;
        if (spawnIndex >= spawnPoints.Length)
            spawnIndex = 0;
    }

    void SpawnNPC()
    {
        Vector2 spawnPos = new Vector2(Random.Range(-20, 40), Random.Range(-60, 50));
        Instantiate(npcPrefab, spawnPos, Quaternion.identity);
    }

    // =========================
    // TRACKING
    // =========================

    public void EnemyDied()
    {
        enemiesAlive--;
        CheckWaveEnd();
    }

    public void NPCFinishedPositive()
    {
        npcsAlive--;
        positiveReviews++;
        CheckWaveEnd();
    }

    public void NPCFinishedNegative()
    {
        npcsAlive--;
        negativeReviews++;
        CheckWaveEnd();
    }

    void CheckWaveEnd()
    {
        if (!spawningFinished)
            return;

        if (enemiesAlive <= 0 && npcsAlive <= 0)
        {
            EnterBuildMode();
        }
    }

    // =========================
    // BUILD MODE
    // =========================

    void EnterBuildMode()
    {
        currentMode = GameMode.Build;

        if (statue != null)
            statue.Heal(50);

        int enemyMoney = enemiesThisWave * 5;
        int positiveBonus = positiveReviews * 10;
        int negativePenalty = negativeReviews * 5;

        AddMoney(enemyMoney + positiveBonus - negativePenalty);

        if (startWaveButton != null)
            startWaveButton.SetActive(true);
    }

    // =========================
    // NEXT WAVE
    // =========================

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
    // WAVE COUNT SYSTEM
    // =========================

    int GetEnemyCountForWave(int wave)
    {
        // FIXED WAVES
        if (wave == 1) return 10;
        if (wave == 2) return 15;
        if (wave == 3) return 30;

        // NIGHT WAVE CHECK
        bool isNightWaveLocal = (wave % 3 == 0);

        float baseValue = wave * 10f;
        float multiplier;

        if (isNightWaveLocal)
            multiplier = Random.Range(1.3f, 1.75f);
        else
            multiplier = Random.Range(1.0f, 1.25f);

        return Mathf.CeilToInt(baseValue * multiplier);
    }

    int GetNPCCountForWave(int wave)
    {
        if (wave == 1) return 1;
        if (wave == 2) return 2;
        if (wave == 3) return 3;

        return 1;
    }

    // =========================
    // DEBUG
    // =========================

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backslash))
        {
            AddMoney(1000);
        }
    }

}