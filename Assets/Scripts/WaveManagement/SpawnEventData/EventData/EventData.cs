using UnityEngine;

public abstract class EventData : SpawnData // we must subclass this object to use it
{
    [Header("Event Data")]
    [Range(0f, 1f)] public float probability = 1f;
    [Range(0f, 1f)] public float luckFactor = 1f;

    [Tooltip("If a value is specified, this event will only occur after the level runs for this number of seconds.")]
    public float activeAfter = 0f;

    public abstract bool Activate(Player player = null, bool alwaysFires = false);

    public bool IsActive()
    {
        if (!GameController.Instance) return false;
        if (GameController.Instance.GetElapsedTime() > activeAfter) return true;
        return false;
    }

    public bool CheckIfEventWillOccur(Player player)
    {
        if (probability >= 1) return true;

        if (probability / Mathf.Max(1, (player.ActualStats.luck)) >= Random.Range(0f, 1f)) return true; // high luck should mean a lower chance that events happen

        return false;
    }
}
