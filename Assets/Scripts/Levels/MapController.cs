using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    public List<GameObject> terrainChunks;
    public GameObject player;
    public GameObject chunksParent;
    public float checkerRadius;
    public float chunkWidth;
    public LayerMask terrainMask;
    [HideInInspector]
    public GameObject currentChunk;

    PlayerMovement pm;

    [Header("Optimization")]
    public List<GameObject> spawnedChunks;
    GameObject latestChunk;
    public float maxOptimizationDistance; // must be greater than the length and width of tilemaps
    float optimizationDistance;
    float optimizerCooldown;
    public float optimizerCooldownDuration;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pm = FindAnyObjectByType<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        BuildNewChunks();
        ChunkOptimizer();
    }

    void BuildNewChunks()
    {
        // check player movement direction
        // grab the current chunk and skip forwards in the direction the player is moving
        // then spawn the chunk in that location

        // TODO: What happens if you move RIGHT UP and come to a corner?
        // Where should you place the chunk? We should probably cast multiple vectors?
        // Probably just perform this at all times, checking in all directions

        // Check https://blog.terresquall.com/community/topic/part-2-5-map-generation-on-roids-greatly-improved/ for improvements

        Vector3 chunkSpawnPosition;

        if (currentChunk == null)
        {
            Debug.Log("No chunk data available");
            return;
        }

        if (pm.movementDirection.x > 0 && pm.movementDirection.y == 0)              // RIGHT
        {
            // if there is no chunk in that specific direction, spawn a new one!
            if (!Physics2D.OverlapCircle(player.transform.position + new Vector3(chunkWidth, 0, 0), checkerRadius, terrainMask))
            {
                chunkSpawnPosition = currentChunk.transform.position + new Vector3(chunkWidth, 0, 0);
                SpawnChunk(chunkSpawnPosition);
            }
        }
        else if (pm.movementDirection.x < 0 && pm.movementDirection.y == 0)              // LEFT
        {
            if (!Physics2D.OverlapCircle(player.transform.position + new Vector3(-chunkWidth, 0, 0), checkerRadius, terrainMask))
            {
                chunkSpawnPosition = currentChunk.transform.position + new Vector3(-chunkWidth, 0, 0);
                SpawnChunk(chunkSpawnPosition);
            }
        }
        else if (pm.movementDirection.x == 0 && pm.movementDirection.y > 0)              // UP
        {
            if (!Physics2D.OverlapCircle(player.transform.position + new Vector3(0, chunkWidth, 0), checkerRadius, terrainMask))
            {
                chunkSpawnPosition = currentChunk.transform.position + new Vector3(0, chunkWidth, 0);
                SpawnChunk(chunkSpawnPosition);
            }
        }
        else if (pm.movementDirection.x == 0 && pm.movementDirection.y < 0)              // DOWN
        {
            if (!Physics2D.OverlapCircle(player.transform.position + new Vector3(0, -chunkWidth, 0), checkerRadius, terrainMask))
            {
                chunkSpawnPosition = currentChunk.transform.position + new Vector3(0, -chunkWidth, 0);
                SpawnChunk(chunkSpawnPosition);
            }
        }
        else if (pm.movementDirection.x > 0 && pm.movementDirection.y > 0)              // RIGHT UP
        {
            if (!Physics2D.OverlapCircle(player.transform.position + new Vector3(chunkWidth, chunkWidth, 0), checkerRadius, terrainMask))
            {
                chunkSpawnPosition = currentChunk.transform.position + new Vector3(chunkWidth, chunkWidth, 0);
                SpawnChunk(chunkSpawnPosition);
            }
        }
        else if (pm.movementDirection.x > 0 && pm.movementDirection.y < 0)              // RIGHT DOWN
        {
            if (!Physics2D.OverlapCircle(player.transform.position + new Vector3(chunkWidth, -chunkWidth, 0), checkerRadius, terrainMask))
            {
                chunkSpawnPosition = currentChunk.transform.position + new Vector3(chunkWidth, -chunkWidth, 0);
                SpawnChunk(chunkSpawnPosition);
            }
        }
        else if (pm.movementDirection.x < 0 && pm.movementDirection.y > 0)              // LEFT UP
        {
            if (!Physics2D.OverlapCircle(player.transform.position + new Vector3(-chunkWidth, chunkWidth, 0), checkerRadius, terrainMask))
            {
                chunkSpawnPosition = currentChunk.transform.position + new Vector3(-chunkWidth, chunkWidth, 0);
                SpawnChunk(chunkSpawnPosition);
            }
        }
        else if (pm.movementDirection.x < 0 && pm.movementDirection.y < 0)              // LEFT DOWN
        {
            if (!Physics2D.OverlapCircle(player.transform.position + new Vector3(-chunkWidth, -chunkWidth, 0), checkerRadius, terrainMask))
            {
                chunkSpawnPosition = currentChunk.transform.position + new Vector3(-chunkWidth, -chunkWidth, 0);
                SpawnChunk(chunkSpawnPosition);
            }
        }
    }

    void SpawnChunk(Vector3 spawnPosition)
    {
        int rand = Random.Range(0, terrainChunks.Count);
        latestChunk = Instantiate(terrainChunks[rand], spawnPosition, Quaternion.identity);
        latestChunk.transform.parent = chunksParent.transform;

        spawnedChunks.Add(latestChunk);
    }

    void ChunkOptimizer()
    {
        optimizerCooldown -= Time.deltaTime;

        if (optimizerCooldown > 0f)
        {
            return;
        }

        optimizerCooldown = optimizerCooldownDuration;

        foreach (GameObject chunk in spawnedChunks)
        {
            optimizationDistance = Vector3.Distance(player.transform.position, chunk.transform.position);
            if (optimizationDistance > maxOptimizationDistance)
            {
                // disable the object
                chunk.SetActive(false);
            }
            else
            {
                chunk.SetActive(true);
            }
        }
    }
}
