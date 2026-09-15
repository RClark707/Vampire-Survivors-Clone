using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Stats/Weapon Stats")]
public class WeaponStats : ItemStats
{
    [SerializeField]
    GameObject weaponBehavior; // this is the weapon behavior!
    public GameObject WeaponBehavior { get => weaponBehavior; private set => weaponBehavior = value; }

    // [SerializeField]
    // WeaponStats evolvedStats; // the stats of the evolved form of the weapon
    // public WeaponStats EvolvedStats { get => evolvedStats; private set => evolvedStats = value; }

    [SerializeField]
    PassiveStats catalystPassive; // the passive required to evolve
    public PassiveStats CatalystPassive { get => catalystPassive; private set => catalystPassive = value; }

    [SerializeField]
    GameObject evolvedWeaponController; // the weapon controller used by the evolved weapon
    public GameObject EvolvedWeaponController { get => evolvedWeaponController; private set => evolvedWeaponController = value; }

    [SerializeField]
    float damage = 1;
    public float Damage { get => damage; private set => damage = value; }

    [SerializeField]
    float projectileSpeed = 10;
    public float ProjectileSpeed { get => projectileSpeed; private set => projectileSpeed = value; }

    [SerializeField]
    float cooldown = 2;
    public float Cooldown { get => cooldown; private set => cooldown = value; }

    [SerializeField]
    int pierceCount;
    public int PierceCount { get => pierceCount; private set => pierceCount = value; }
}
