using System;
using UnityEngine;

[Obsolete("We have replaced Enemy stats with structs in the base class.")]
public class EnemyStats : EntityStats
{
    [SerializeField]
    float damage;
    public float Damage { get => damage; private set => damage = value; }
}
