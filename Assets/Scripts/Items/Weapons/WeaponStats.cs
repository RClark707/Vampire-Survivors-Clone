using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Stats/Weapon Stats")]
public class WeaponStats : ItemStats
{
    [SerializeField]
    GameObject weaponPrefab;
    public GameObject WeaponPrefab { get => weaponPrefab; private set => weaponPrefab = value; }

    [SerializeField]
    WeaponStats evolvedStats;
    public WeaponStats EvolvedStats { get => evolvedStats; private set => evolvedStats = value; }

    [SerializeField]
    GameObject evolutionPrefab;
    public GameObject EvolutionPrefab { get => evolutionPrefab; private set => evolutionPrefab = value; }

    [SerializeField]
    float damage = 1;
    public float Damage { get => damage; private set => damage = value; }

    [SerializeField]
    float speed = 10;
    public float Speed { get => speed; private set => speed = value; }

    [SerializeField]
    float cooldown = 2;
    public float Cooldown { get => cooldown; private set => cooldown = value; }

    [SerializeField]
    int pierceCount;
    public int PierceCount { get => pierceCount; private set => pierceCount = value; }
}
