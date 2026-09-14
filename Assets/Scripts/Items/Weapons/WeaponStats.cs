using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Stats/Weapon Stats")]
public class WeaponStats : ItemStats
{
    [SerializeField]
    GameObject weaponPrefab; // this is the weapon behavior!
    public GameObject WeaponPrefab { get => weaponPrefab; private set => weaponPrefab = value; }

    // [SerializeField]
    // WeaponStats evolvedStats; // the stats of the evolved form of the weapon
    // public WeaponStats EvolvedStats { get => evolvedStats; private set => evolvedStats = value; }

    [SerializeField]
    WeaponController evolvedWeaponController; // the weapon controller used by the evolved weapon
    public WeaponController EvolvedWeaponController { get => evolvedWeaponController; private set => evolvedWeaponController = value; }

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
