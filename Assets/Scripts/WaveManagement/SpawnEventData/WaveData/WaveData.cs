using UnityEngine;

[CreateAssetMenu(fileName = "New Wave Data", menuName = "Spawn Data/Wave Data")]
public class WaveData : SpawnData
{
    [Header("Wave Data")]

    [Tooltip("If there are less than this number of enemies, we will keep spawning until we get there.")]
    [Min(0)] public int initialMinimumCount; // this could use a rename probably

    [Tooltip("How many enemies can this wave spawn at a maximum?")]
    [Min(1)] public uint totalSpawns = uint.MaxValue;

    [System.Flags] public enum ExitCondition { WaveDuration = 1, ReachedTotalSpawns = 2 }
    [Tooltip("What conditions can trigger the end of this wave?")]
    public ExitCondition exitCondition = (ExitCondition)1;

    [Tooltip("All enemies must be dead for the wave to advance.")]
    public bool mustKillAllEnemies = false;

    [HideInInspector] public uint spawnCount;

    public override int GetCount(int totalEnemies = 0)
    {
        int count = Random.Range(spawnsPerTick.x, spawnsPerTick.y);

        if (totalEnemies + count < initialMinimumCount)
        {
            count = initialMinimumCount - totalEnemies; // we try to always hit the minimum number of enemies
        }

        return count;
    }

}
