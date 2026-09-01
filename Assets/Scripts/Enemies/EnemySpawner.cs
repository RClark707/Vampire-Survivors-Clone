using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public string name;
        public List<EnemyGroup> enemyGroups;
        public float spawnInterval;
        public int totalEnemies; // total number of enemies to be spawned
        [HideInInspector]
        public int spawnCount; // how many have we spawned so far

        [System.Serializable]
        public class EnemyGroup
        {
            public string name;
            public int totalEnemies; // how many should we spawn
            [HideInInspector]
            public int spawnCount; // how many have we spawned so far
            public GameObject enemyPrefab;

        }
    }

    [Header("Wave Management")]
    public List<Wave> waves;
    public int currentWaveIndex = 0;
    public float distanceToSpawn = 25f;
    Transform playerTransform;

    [Header("Spawner Attributes")]
    float spawnTimer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerTransform = FindAnyObjectByType<Player>().transform;
        GetTotalEnemiesByWave();
        SpawnEnemies();
    }

    // Update is called once per frame
    void Update()
    {

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

    void SpawnEnemies() // this is to be called multiple times, but not by a for loop?
    {
        Wave currentWave = waves[currentWaveIndex];

        if (currentWave.spawnCount < currentWave.totalEnemies) // then keep spawning
        {
            foreach (var enemyGroup in currentWave.enemyGroups)
            {
                if (enemyGroup.spawnCount < enemyGroup.totalEnemies) // again, keep spawning
                {
                    Vector2 spawnPosition = GetRandomSpawnPosition();
                    Instantiate(enemyGroup.enemyPrefab, spawnPosition, Quaternion.identity);

                    enemyGroup.spawnCount++;
                    currentWave.spawnCount++;
                }
            }
        }
    }

    Vector2 GetRandomSpawnPosition()
    {
        return Random.onUnitSphere * distanceToSpawn;
    }
}
