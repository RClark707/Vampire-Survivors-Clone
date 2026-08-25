using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    public List<GameObject> terrainChunks;
    public GameObject player;
    public float checkerRadius;
    public float chunkWidth;
    public LayerMask terrainMask;

    PlayerMovement pm;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pm = FindAnyObjectByType<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        ChunkChecker();
    }

    void ChunkChecker()
    {
        // check player movement direction
        // grab the current chunk and skip forwards in the direction the player is moving
        // then spawn the chunk in that location

        Vector3 chunkSpawnPosition;

        if (pm.movementDirection.x > 0 && pm.movementDirection.y == 0)              // RIGHT
        {
            // if there is no chunk in that specific direction, spawn a new one!
            if (!Physics2D.OverlapCircle(player.transform.position + new Vector3(chunkWidth, 0, 0), checkerRadius, terrainMask))
            {
                if (pm.currentChunk == null) { Debug.Log("No chunk data available"); return; }
                chunkSpawnPosition = pm.currentChunk.transform.position + new Vector3(chunkWidth, 0, 0);
                SpawnChunk(chunkSpawnPosition);
            }
        }
        else if (pm.movementDirection.x < 0 && pm.movementDirection.y == 0)              // LEFT
        {
            // if there is no chunk in that specific direction, spawn a new one!
            if (!Physics2D.OverlapCircle(player.transform.position + new Vector3(-chunkWidth, 0, 0), checkerRadius, terrainMask))
            {
                if (pm.currentChunk == null) { Debug.Log("No chunk data available"); return; }
                chunkSpawnPosition = pm.currentChunk.transform.position + new Vector3(-chunkWidth, 0, 0);
                SpawnChunk(chunkSpawnPosition);
            }
        }
        else if (pm.movementDirection.x == 0 && pm.movementDirection.y > 0)              // UP
        {
            // if there is no chunk in that specific direction, spawn a new one!
            if (!Physics2D.OverlapCircle(player.transform.position + new Vector3(0, chunkWidth, 0), checkerRadius, terrainMask))
            {
                if (pm.currentChunk == null) { Debug.Log("No chunk data available"); return; }
                chunkSpawnPosition = pm.currentChunk.transform.position + new Vector3(0, chunkWidth, 0);
                SpawnChunk(chunkSpawnPosition);
            }
        }
        else if (pm.movementDirection.x == 0 && pm.movementDirection.y < 0)              // DOWN
        {
            // if there is no chunk in that specific direction, spawn a new one!
            if (!Physics2D.OverlapCircle(player.transform.position + new Vector3(0, -chunkWidth, 0), checkerRadius, terrainMask))
            {
                if (pm.currentChunk == null) { Debug.Log("No chunk data available"); return; }
                chunkSpawnPosition = pm.currentChunk.transform.position + new Vector3(0, -chunkWidth, 0);
                SpawnChunk(chunkSpawnPosition);
            }
        }
        else if (pm.movementDirection.x > 0 && pm.movementDirection.y > 0)              // RIGHT UP
        {
            // if there is no chunk in that specific direction, spawn a new one!
            if (!Physics2D.OverlapCircle(player.transform.position + new Vector3(chunkWidth, chunkWidth, 0), checkerRadius, terrainMask))
            {
                if (pm.currentChunk == null) { Debug.Log("No chunk data available"); return; }
                chunkSpawnPosition = pm.currentChunk.transform.position + new Vector3(chunkWidth, chunkWidth, 0);
                SpawnChunk(chunkSpawnPosition);
            }
        }
        else if (pm.movementDirection.x > 0 && pm.movementDirection.y < 0)              // RIGHT DOWN
        {
            // if there is no chunk in that specific direction, spawn a new one!
            if (!Physics2D.OverlapCircle(player.transform.position + new Vector3(chunkWidth, -chunkWidth, 0), checkerRadius, terrainMask))
            {
                if (pm.currentChunk == null) { Debug.Log("No chunk data available"); return; }
                chunkSpawnPosition = pm.currentChunk.transform.position + new Vector3(chunkWidth, -chunkWidth, 0);
                SpawnChunk(chunkSpawnPosition);
            }
        }
        else if (pm.movementDirection.x < 0 && pm.movementDirection.y > 0)              // LEFT UP
        {
            // if there is no chunk in that specific direction, spawn a new one!
            if (!Physics2D.OverlapCircle(player.transform.position + new Vector3(-chunkWidth, chunkWidth, 0), checkerRadius, terrainMask))
            {
                if (pm.currentChunk == null) { Debug.Log("No chunk data available"); return; }
                chunkSpawnPosition = pm.currentChunk.transform.position + new Vector3(-chunkWidth, chunkWidth, 0);
                SpawnChunk(chunkSpawnPosition);
            }
        }
        else if (pm.movementDirection.x < 0 && pm.movementDirection.y < 0)              // LEFT DOWN
        {
            // if there is no chunk in that specific direction, spawn a new one!
            if (!Physics2D.OverlapCircle(player.transform.position + new Vector3(-chunkWidth, -chunkWidth, 0), checkerRadius, terrainMask))
            {
                if (pm.currentChunk == null) { Debug.Log("No chunk data available"); return; }
                chunkSpawnPosition = pm.currentChunk.transform.position + new Vector3(-chunkWidth, -chunkWidth, 0);
                SpawnChunk(chunkSpawnPosition);
            }
        }
    }

    void SpawnChunk(Vector3 spawnPosition)
    {
        int rand = Random.Range(0, terrainChunks.Count);
        GameObject chunk = Instantiate(terrainChunks[rand], spawnPosition, Quaternion.identity);
        // change the chunk's parent here
    }
}
