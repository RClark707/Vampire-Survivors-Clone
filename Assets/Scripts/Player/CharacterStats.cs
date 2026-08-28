using UnityEngine;

[CreateAssetMenu(fileName = "New Character Stats", menuName = "Stats/Character Stats")]
public class CharacterStats : EntityStats
{
    [SerializeField]
    GameObject startingWeapon;
    public GameObject StartingWeapon { get => startingWeapon; private set => startingWeapon = value; }

    [SerializeField]
    float recovery;
    public float Recovery { get => recovery; private set => recovery = value; }

    [SerializeField]
    float armor;
    public float Armor { get => armor; private set => armor = value; }

    [SerializeField]
    float might;
    public float Might { get => might; private set => might = value; }

    [SerializeField]
    float projectileSpeed;
    public float ProjectileSpeed { get => projectileSpeed; private set => projectileSpeed = value; }

    [SerializeField]
    float area;
    public float Area { get => area; private set => area = value; }

    [SerializeField]
    float magnet;
    public float Magnet { get => magnet; private set => magnet = value; }

    [SerializeField]
    float growth;
    public float Growth { get => growth; private set => growth = value; }

    [SerializeField]
    float luck;
    public float Luck { get => luck; private set => luck = value; }
}
