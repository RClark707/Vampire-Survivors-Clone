using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Stats", menuName = "Stats/Enemy Stats")]
public class EnemyStats : EntityStats
{
    [SerializeField]
    float damage;
    public float Damage { get => damage; private set => damage = value; }
}
