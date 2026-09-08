using UnityEngine;

public class DestructibleProp : Entity
{
    [Header("Prop Stats")]
    public PropStats stats;

    public override void Awake()
    {
        name = stats.name;
        health = stats.MaxHealth;
        base.Awake();
    }
}
