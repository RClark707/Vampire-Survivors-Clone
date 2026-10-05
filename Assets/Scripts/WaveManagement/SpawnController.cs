using UnityEngine;

public class SpawnController : MonoBehaviour
{
    int currentWaveIndex;
    int currentWaveSpawnCount = 0;

    public WaveData[] waveData;
    public Camera referenceCamera;

    [Tooltip("If there are more than this number of enemies, stops spawning any more.")]
    public int maximumEnemyCount = 300;
    float spawnTimer;
    float currentWaveDuration = 0f;

    public static SpawnController Instance;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        spawnTimer -= Time.deltaTime;
        currentWaveDuration += Time.deltaTime;

        if (spawnTimer <= 0f)
        {
            if (HasWaveEnded())
            {
                currentWaveIndex++; // this will trigger the next wave
                currentWaveDuration = currentWaveSpawnCount = 0; // set both of these back to 0

                if (currentWaveIndex >= waveData.Length)
                {
                    Debug.Log("All waves have been spawned. Shutting down.", this);
                    enabled = false;
                }

                return;
            }

            if (!CanSpawn()) // if we cannot spawn new enemies at all
            {
                spawnTimer += waveData[currentWaveIndex].GetSpawnInterval(); // resets the spawn timer
                return;
            }

            GameObject[] spawns = waveData[currentWaveIndex].GetSpawns(); // Enemy.count

            foreach (GameObject prefab in spawns)
            {
                if (!CanSpawn()) continue; // just skip ahead if we can't spawn yet (this happens if a previous loop iteration brought us to the limit

                Instantiate(prefab, GenerateSpawnPosition(), Quaternion.identity);

                currentWaveSpawnCount++;
            }

            spawnTimer += waveData[currentWaveIndex].GetSpawnInterval(); // resets the spawn timer
        }
    }

    void Reset()
    {
        referenceCamera = Camera.main;
    }

    public bool HasWaveEnded()
    {
        WaveData currentWave = waveData[currentWaveIndex];

        if ((currentWave.exitCondition & WaveData.ExitCondition.WaveDuration) > 0)
        {
            if (currentWaveDuration < currentWave.duration) return false;
        }

        if ((currentWave.exitCondition & WaveData.ExitCondition.ReachedTotalSpawns) > 0)
        {
            if (currentWaveSpawnCount < currentWave.totalSpawns) return false;
        }

        if (currentWave.mustKillAllEnemies && Enemy.count > 0)
        {
            return false;
        }

        return true;
    }

    public bool CanSpawn()
    {
        if (HasExceededMaxEnemies()) return false;

        if (Instance.currentWaveSpawnCount > Instance.waveData[Instance.currentWaveIndex].totalSpawns) return false;

        if (Instance.currentWaveDuration > Instance.waveData[Instance.currentWaveIndex].duration) return false;

        return true;
    }

    public static bool HasExceededMaxEnemies()
    {
        if (!Instance) return false;

        if (Enemy.count > Instance.maximumEnemyCount) return true;

        return false;
    }

    public static Vector3 GenerateSpawnPosition()
    {
        if (!Instance.referenceCamera) Instance.referenceCamera = Camera.main;

        if (!Instance.referenceCamera.orthographic)
        {
            Debug.LogWarning("The reference camera is not orthogonal! This will cause enemy spawns to sometimes appear within camera boundaries.");
        }

        float xPos = Random.Range(0f, 1f), yPos = Random.Range(0f, 1f);

        switch (Random.Range(0, 2))
        {
            case 0:
            default:
                return Instance.referenceCamera.ViewportToWorldPoint(new Vector3(Mathf.Round(xPos), yPos));
            case 1:
                return Instance.referenceCamera.ViewportToWorldPoint(new Vector3(xPos, Mathf.Round(yPos)));
        }
    }

    public static bool IsWithinBoundaries(Transform checkedObject)
    {
        Camera c = Instance && Instance.referenceCamera ? Instance.referenceCamera : Camera.main;

        Vector2 viewport = c.WorldToViewportPoint(checkedObject.position);
        if (viewport.x < 0f || viewport.x > 1f) return false;
        if (viewport.y < 0f || viewport.y > 1f) return false;
        return true;
    }
}
