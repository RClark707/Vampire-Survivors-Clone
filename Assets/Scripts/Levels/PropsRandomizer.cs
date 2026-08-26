using System.Collections.Generic;
using UnityEngine;

public class PropsRandomizer : MonoBehaviour
{
    public List<GameObject> propSpawnLocations;
    public List<GameObject> propPrefabs;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpawnProps();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void SpawnProps()
    {
        foreach (GameObject spawnLocation in propSpawnLocations)
        {
            // This is where we can randomly decide what to spawn, change from terrain to interactable objects too
            // Choose a random prop
            int rand = Random.Range(0, propPrefabs.Count);
            GameObject prop = Instantiate(propPrefabs[rand], spawnLocation.transform.position, Quaternion.identity);
            prop.transform.parent = spawnLocation.transform;
        }
    }
}
