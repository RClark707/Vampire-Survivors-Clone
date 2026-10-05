using UnityEngine;

[CreateAssetMenu(fileName = "New Charger Event", menuName = "Spawn Data/Charger Event")]
public class ChargerEventData : EventData
{
    [Header("Charger Event Data")]
    [Range(0f, 360f)] public float possibleAngles = 360f;
    [Min(0)] public float spawnRadius = 2f, spawnDistance = 20f;

    public override bool Activate(Player player = null, bool alwaysFires = false)
    {
        if (player)
        {
            float randomAngle = Random.Range(0, possibleAngles) * Mathf.Deg2Rad;
            foreach (GameObject go in GetSpawns())
            {
                Instantiate(go, player.transform.position + new Vector3(
                    (spawnDistance + Random.Range(-spawnRadius, spawnRadius)) * Mathf.Cos(randomAngle),
                    (spawnDistance + Random.Range(-spawnRadius, spawnRadius)) * Mathf.Sin(randomAngle)
                    ), Quaternion.identity);
            }
        }

        return false;
    }
}
