using UnityEngine;

[CreateAssetMenu(fileName = "New Passive Stats", menuName = "Stats/Passive Stats")]
public class PassiveStats : ItemStats
{
    [SerializeField]
    PlayerUpgradeStats stat;
    public PlayerUpgradeStats Stat { get => stat; protected set => stat = value; }

    [SerializeField]
    float multiplier;
    public float Multiplier { get => multiplier; protected set => multiplier = value; }
}
