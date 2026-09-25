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

        //public Stats(
        //    float maxHealth = 100f, float recovery = 0f, float armor = 0f,
        //    float moveSpeed = 1f, float might = 1f, float area = 30f,
        //    float projectileSpeed = 1f, float duration = 1f,
        //    int amount = 0,
        //    float cooldown = 1f,
        //    float luck = 1f, float growth = 1f, float greed = 1f, float curse = 1f,
        //    float magnet = 30f,
        //    int revival = 1
        //    )
        //{
        //    this.maxHealth = maxHealth;
        //    this.recovery = recovery;
        //    this.armor = armor;
        //    this.moveSpeed = moveSpeed;
        //    this.might = might;
        //    this.area = area;
        //    this.projectileSpeed = projectileSpeed;
        //    this.duration = duration;
        //    this.amount = amount;
        //    this.cooldown = cooldown;
        //    this.luck = luck;
        //    this.growth = growth;
        //    this.greed = greed;
        //    this.curse = curse;
        //    this.magnet = magnet;
        //    this.revival = revival;
        //}

        public static Stats operator +(Stats s1, Stats s2) // important to recognize this functionality, since all many stats are used as percentages
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
    }

    public Stats stats = new Stats
    {
        maxHealth = 100f,
        moveSpeed = 5f,
        might = 1f,
        area = 10f,
        projectileSpeed = 1f,
        duration = 1f,
        cooldown = 1f,
        luck = 1f,
        growth = 1f,
        greed = 1f,
        curse = 1f,
    };
}
