using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemySpawner.Wave;

public class EnemySpawner : MonoBehaviour
{
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
            // [HideInInspector]
            public int totalEnemies; // how many should we spawn
            [HideInInspector]
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
    Player player;

    [Header("Spawner Attributes")]
    public int enemyCount;
    public int maxEnemies;
    public Transform enemyParent;
    bool canSpawn = true;

    private void Start()
    {
        player = FindAnyObjectByType<Player>();
        GetTotalEnemiesByWave();
        HandleEnemyGroupCoroutines();
    }

    void Update()
    {
        // have we spawned all the enemies from the current wave? 
        if (waves[currentWaveIndex].spawnCount == waves[currentWaveIndex].totalEnemies)
        {
            StartCoroutine(BeginNextWave()); // if so, then begin the next wave
        }
    }

    IEnumerator SpawnTimer(EnemyGroup eg, int amount = 1)
    {
        WaitForSeconds delay = new WaitForSeconds(waves[currentWaveIndex].spawnInterval);

        while (eg.spawnCount < eg.totalEnemies)
        {
            // Debug.Log($"Spawn interval timer has elapsed! Spawning new enemies.");
            SpawnEnemies(eg, amount);
            yield return delay;
        }
    }

    IEnumerator BeginNextWave()
    {
        yield return new WaitForSeconds(2);

        //Debug.Log($"{waves[currentWaveIndex].name} has ended.");

        if (currentWaveIndex < waves.Count - 1)
        {
            currentWaveIndex++;
        }
        else // at some point we want to just loop back to the first wave
        // as is, this won't spawn any new enemies until the original ones are killed. I guess this is OK for now
        // TODO: Reconsider the desired functionality of resetting the waves.
        {
            // reset the enemy groups for all waves
            currentWaveIndex = 0;
            //Debug.Log($"Let's take it from the top at Wave {currentWaveIndex}");
            foreach (Wave wave in waves)
            {
                wave.spawnCount = 0;
                foreach (EnemyGroup eg in wave.enemyGroups)
                {
                    eg.spawnCount = 0;
                }
            }
        }

        //Debug.Log($"{waves[currentWaveIndex].name} is about to begin!");
        GetTotalEnemiesByWave();
        HandleEnemyGroupCoroutines();
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
                    if (eg.isCircularWave)
                    {
                        StartCoroutine(SpawnTimer(eg, eg.totalEnemies)); // spawn the entire enemy group in a circle
                    }
                    else
                    {
                        StartCoroutine(SpawnTimer(eg));
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

        foreach (EnemyGroup group in currentWave.enemyGroups)
        {
            total += group.totalEnemies;
        }

        currentWave.totalEnemies = total;
        // Debug.Log($"The total enemies for this wave is {total}");
    }

    void SpawnEnemy(EnemyGroup eg, Vector2 spawnPos, bool isElite = false)
    {
        if (!canSpawn && !isElite) return; // don't spawn if we are over our limit and it isn't an elite enemy

        // Debug.Log($"Spawning a {eg.enemyPrefab.name} at {spawnPos}");
        GameObject go = Instantiate(eg.enemyPrefab, spawnPos, Quaternion.identity);
        go.transform.SetParent(enemyParent);
        eg.spawnCount++;
        waves[currentWaveIndex].spawnCount++;
        enemyCount++;

        if (enemyCount >= maxEnemies)
        {
            canSpawn = false;
        }
        // here you can factor code to handle assigning any variables for the enemy or making it elite
    }

    Vector2 GetRandomSpawnPosition()
    {
        Vector2 offset = (new Vector2(0, distanceToSpawn)) - (Vector2)player.transform.position;
        Vector2 spawnPosition = Quaternion.Euler(0, 0, Random.Range(0, 360)) * offset; // grab a random rotation

        return spawnPosition + (Vector2)player.transform.position;
    }

    void SpawnEnemies(EnemyGroup eg, int amount)
    {
        // Debug.Log($"About to spawn {amount} {eg.enemyPrefab.name}.");
        for (int i = 0; i < amount; i++)
        {
            SpawnEnemy(eg, GetRandomSpawnPosition());
        }
    }

    public void OnEnemyKilled()
    {
        enemyCount--;

        if (enemyCount < maxEnemies)
        {
            canSpawn = true;
        }
    }
}
