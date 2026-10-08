using UnityEngine;

[CreateAssetMenu(fileName = "New Character Stats", menuName = "Stats/Character Stats B")]
public class CharacterStatsB : ScriptableObject
{
    [SerializeField]
    Sprite icon;
    public Sprite Icon { get => icon; set => icon = value; }

    [SerializeField]
    new string name;
    public string Name { get => name; set => name = value; }
    [SerializeField]
    string fullName;
    public string FullName { get => fullName; set => fullName = value; }
    [SerializeField]
    string description;
    public string Description { get => description; set => description = value; }

    [SerializeField]
    WeaponStatsB startingWeapon;
    public WeaponStatsB StartingWeapon { get => startingWeapon; set => startingWeapon = value; }

    [System.Serializable]
    public struct Stats
    {
        public float maxHealth, recovery, armor;
        [Range(-1, 10)] public float moveSpeed, might, area;
        [Range(-1, 5)] public float projectileSpeed, duration;
        [Range(-1, 10)] public int amount;
        [Range(-1, 1)] public float cooldown;
        [Min(-1)] public float luck, growth, greed, curse;
        public float magnet;
        public int revival;

        public static Stats operator +(Stats s1, Stats s2)
        {
            s1.maxHealth += s2.maxHealth;
            s1.recovery += s2.recovery;
            s1.armor += s2.armor;
            s1.moveSpeed += s2.moveSpeed;
            s1.might += s2.might;
            s1.area += s2.area;
            s1.projectileSpeed += s2.projectileSpeed;
            s1.duration += s2.duration;
            s1.amount += s2.amount;
            s1.cooldown += s2.cooldown;
            s1.luck += s2.luck;
            s1.growth += s2.growth;
            s1.greed += s2.greed;
            s1.curse += s2.curse;
            s1.magnet += s2.magnet;
            s1.revival += s2.revival;
            return s1;
        }

        public static Stats operator *(Stats s1, Stats s2)
        {
            s1.maxHealth *= s2.maxHealth;
            s1.recovery *= s2.recovery;
            s1.armor *= s2.armor;
            s1.moveSpeed *= s2.moveSpeed;
            s1.might *= s2.might;
            s1.area *= s2.area;
            s1.projectileSpeed *= s2.projectileSpeed;
            s1.duration *= s2.duration;
            s1.amount *= s2.amount;
            s1.cooldown *= s2.cooldown;
            s1.luck *= s2.luck;
            s1.growth *= s2.growth;
            s1.greed *= s2.greed;
            s1.curse *= s2.curse;
            s1.magnet *= s2.magnet;
            s1.revival *= s2.revival;
            return s1;
        }
    }

    public Stats stats = new Stats
    {
        maxHealth = 100f,
        moveSpeed = 1f,
        might = 1f,
        area = 10f,
        projectileSpeed = 1f,
        duration = 1f,
        cooldown = 1f,
        luck = 1f,
        growth = 1f,
        greed = 1f,
        curse = 1f
    };
}
