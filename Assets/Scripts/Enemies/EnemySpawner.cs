using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemySpawner.Wave;

public class EnemySpawner : MonoBehaviour
{
    // // Start is called once before the first execution of Update after the MonoBehaviour is created
    // void Start()
    // {
    //     playerTransform = FindAnyObjectByType<Player>().transform;
    //     GetTotalEnemiesByWave();
    // }
    // 
    // // Update is called once per frame
    // void Update()
    // {
    //     // do we have waves left, and has the wave started already?
    //     if (currentWaveIndex < waves.Count - 1 && waves[currentWaveIndex].spawnCount == 0)
    //     {
    //         StartCoroutine(BeginNextWave());
    //     }
    // 
    //     spawnTimer += Time.deltaTime;
    // 
    //     if (spawnTimer > waves[currentWaveIndex].spawnInterval)
    //     {
    //         spawnTimer = 0f;
    //         SpawnEnemies();
    //     }
    // }
    // 
    // IEnumerator BeginNextWave()
    // {
    //     // wait for specific time
    //     yield return new WaitForSeconds(waveInterval);
    // 
    //     if (currentWaveIndex < waves.Count - 1)
    //     {
    //         currentWaveIndex++;
    //         GetTotalEnemiesByWave();
    //     }
    // }
    // 
    // void GetTotalEnemiesByWave()
    // {
    //     int total = 0;
    //     Wave currentWave = waves[currentWaveIndex];
    // 
    //     foreach (var group in currentWave.enemyGroups)
    //     {
    //         total += group.totalEnemies;
    //     }
    // 
    //     currentWave.totalEnemies = total;
    //     Debug.Log($"The total enemies for this wave is {total}");
    // }
    // 
    // void SpawnEnemies() // this is to be called multiple times, but not by a for loop?
    // {
    //     Wave currentWave = waves[currentWaveIndex];
    // 
    //     if (currentWave.spawnCount < currentWave.totalEnemies && !maxEnemiesReached) // then keep spawning
    //     {
    //         foreach (var enemyGroup in currentWave.enemyGroups)
    //         {
    //             if (enemyGroup.spawnCount < enemyGroup.totalEnemies) // again, keep spawning
    //             {
    //                 if (enemyCount >= maxEnemies)
    //                 {
    //                     maxEnemiesReached = true;
    //                     return;
    //                 }
    // 
    //                 Vector2 spawnPosition = GetRandomSpawnPosition();
    //                 Instantiate(enemyGroup.enemyPrefab, spawnPosition, Quaternion.identity);
    // 
    //                 enemyGroup.spawnCount++;
    //                 currentWave.spawnCount++;
    //                 enemyCount++;
    //             }
    //         }
    //     }
    // 
    //     if (enemyCount < maxEnemies)
    //     {
    //         maxEnemiesReached = false;
    //     }
    // }
    // 
    // void SpawnEnemyGroupCircle(Wave.EnemyGroup enemyGroup)
    // {
    //     for (int i = 0; i < enemyGroup.totalEnemies; i++)
    //     {
    // 
    //     }
    // }
    //     

    [System.Serializable]
    public class Wave
    {
        public string name;
        public List<EnemyGroup> enemyGroups;
        public int spawnInterval;
        public int totalEnemies; // total number of enemies to be spawned
        // [HideInInspector]
        public int spawnCount; // how many have we spawned so far

        [System.Serializable]
        public class EnemyGroup
        {
            public string name;
            public int totalEnemies; // how many should we spawn
            // [HideInInspector]
            public int spawnCount; // how many have we spawned so far
            public GameObject enemyPrefab;
            public bool isCircularWave; // Is this a circular group?
            public bool isElite;
        }
    }

    [Header("Wave Management")]
    public List<Wave> waves;
    public int currentWaveIndex = 0;
    public float distanceToSpawn = 25f;
    // Transform playerTransform;

    [Header("Spawner Attributes")]
    // float spawnTimer;
    // public float waveInterval;
    public int enemyCount;
    public int maxEnemies;
    bool canSpawn = true;
    int minutes = 0; // this is only for creating a graphic of the timer
    private int seconds = 0;
    public int Seconds
    {
        get => seconds;
        private set
        {
            seconds = value;
            if (seconds >= 60)
            {
                seconds -= 60;
                minutes += 1;
            }
        }
    }

    private void Start()
    {
        // StartCoroutine(OneSecondTimer());
        GetTotalEnemiesByWave();
        HandleEnemyGroupCoroutines();
    }

    void Update()
    {
        // if (FindObjectsByType<Enemy>(FindObjectsSortMode.None).Length < maxEnemies) // grab the total enemies spawned, this could change
        // {
        //     canSpawn = true;
        // }
        // else
        // {
        //     canSpawn = false;
        // }

        // TODO: This runs before the first wave was spawned...
        if (currentWaveIndex < waves.Count - 1 && waves[currentWaveIndex].spawnCount == 0) // has currentWaveIndex been incremented?
        {
            StartCoroutine(BeginNextWave()); // if so, then begin the next wave
        }
    }

    // The sole job of this function is to track time
    // IEnumerator OneSecondTimer()
    // {
    //     WaitForSeconds delay = new WaitForSeconds(1);
    // 
    //     while (true)
    //     {
    //         Seconds += 1;
    //         yield return delay;
    //     }
    // }

    IEnumerator GroupTimer(EnemyGroup eg, int amount = 1)
    {
        WaitForSeconds delay = new WaitForSeconds(waves[currentWaveIndex].spawnInterval);

        while (eg.spawnCount < eg.totalEnemies)
        {
            Debug.Log($"Spawn interval timer has elapsed! Spawning new enemies.");
            SpawnMultipleEnemies(amount, eg);
            yield return delay;
        }
    }

    IEnumerator BeginNextWave()
    {
        yield return new WaitForSeconds(2);

        if (currentWaveIndex < waves.Count - 1)
        {
            Debug.Log($"{waves[currentWaveIndex].name} has ended.");
            currentWaveIndex++;
            Debug.Log($"{waves[currentWaveIndex].name} is about to begin!");
            GetTotalEnemiesByWave();
            HandleEnemyGroupCoroutines();
        }
    }

    void HandleEnemyGroupCoroutines()
    {
        StopAllCoroutines();

        if (waves[currentWaveIndex].spawnCount < waves[currentWaveIndex].totalEnemies && canSpawn)
        {
            foreach (EnemyGroup eg in waves[currentWaveIndex].enemyGroups)
            {
                if (eg.spawnCount < eg.totalEnemies) // ENEMIES ARE STILL SPAWNING?
                {
                    if (enemyCount >= maxEnemies)
                    {
                        canSpawn = false;
                        return;
                    }

                    if (eg.isCircularWave)
                    {
                        StartCoroutine(GroupTimer(eg, 10)); //eg.totalEnemies
                    }
                    else
                    {
                        StartCoroutine(GroupTimer(eg));
                    }
                }
            }
        }

        if (enemyCount < maxEnemies)
        {
            canSpawn = true;
        }
    }

    void GetTotalEnemiesByWave()
    {
        int total = 0;
        Wave currentWave = waves[currentWaveIndex];

        foreach (var group in currentWave.enemyGroups)
        {
            total += group.totalEnemies;
        }

        currentWave.totalEnemies = total;
        Debug.Log($"The total enemies for this wave is {total}");
    }

    void SpawnEnemy(EnemyGroup eg, Vector2 spawnPos, bool isElite = false)
    {
        if (!canSpawn && !isElite) return; // don't spawn if we are over our limit and it isn't an elite enemy

        Debug.Log($"Spawning a {eg.enemyPrefab.name} at {spawnPos}");
        Instantiate(eg.enemyPrefab, spawnPos, Quaternion.identity);
        eg.spawnCount++;
        waves[currentWaveIndex].spawnCount++;
        enemyCount++;
        // here you can factor code to handle assigning any variables for the enemy or making it elite
    }

    Vector2 GetRandomSpawnPosition()
    {
        return Random.onUnitSphere * distanceToSpawn;
    }

    void SpawnMultipleEnemies(int amount, EnemyGroup eg)
    {
        Debug.Log($"About to spawn {amount} {eg.enemyPrefab.name}.");
        for (int i = 0; i < amount; i++)
        {
            SpawnEnemy(eg, GetRandomSpawnPosition());
        }
    }

    public void OnEnemyKilled()
    {
        enemyCount--;
    }
}
