using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Stats/Weapon Stats")]
public class WeaponStats : ScriptableObject
{
    public new string name;

    [SerializeField]
    GameObject weaponPrefab;
    public GameObject WeaponPrefab { get => weaponPrefab; private set => weaponPrefab = value; }

    [SerializeField]
    float damage;
    public float Damage { get => damage; private set => damage = value; }

    [SerializeField]
    float speed;
    public float Speed { get => speed; private set => speed = value; }

    [SerializeField]
    float cooldown;
    public float Cooldown { get => cooldown; private set => cooldown = value; }

    [SerializeField]
    int pierceCount;
    public int PierceCount { get => pierceCount; private set => pierceCount = value; }
}
