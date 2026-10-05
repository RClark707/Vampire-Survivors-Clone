using UnityEngine;

[CreateAssetMenu(fileName = "New Ring Event", menuName = "Spawn Data/Ring Event")]
public class RingEventData : EventData
{
    [Header("Ring Data")]
    public ParticleSystem spawnEffectPrefab;
    public Vector2 scale = new Vector2(1, 1);
    [Min(0)] public float spawnRadius = 10f, lifespan = 15f;

    public override bool Activate(Player player = null, bool alwaysFires = false)
    {
        if (player)
        {
            GameObject[] spawns = GetSpawns();
            float angleOffset = 2 * Mathf.PI / Mathf.Max(1, spawns.Length);
            float currentAngle = 0f;
            foreach (GameObject go in spawns)
            {
                Vector3 spawnPosition = player.transform.position + new Vector3(
                    spawnRadius * Mathf.Cos(currentAngle) * scale.x,
                    spawnRadius * Mathf.Sin(currentAngle) * scale.y
                    );

                if (spawnEffectPrefab)
                {
                    Instantiate(spawnEffectPrefab, spawnPosition, Quaternion.identity);
                }

                GameObject s = Instantiate(go, spawnPosition, Quaternion.identity);

                if (lifespan > 0) Destroy(s, lifespan);

                currentAngle += angleOffset;
            }
        }

        return false;
    }
}
