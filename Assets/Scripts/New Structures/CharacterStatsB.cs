using UnityEngine;

[CreateAssetMenu(fileName = "New Character Stats", menuName = "Stats/Character Stats B")]
public class CharacterStatsB : MonoBehaviour
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
        public float maxHealth, recovery, moveSpeed, might, projectileSpeed, magnet, area, armor, growth, luck;

        public Stats(float maxHealth = 100f, float recovery = 0f, float moveSpeed = 1f, float might = 1f, float projectileSpeed = 1f, float magnet = 30f,
            float area = 30f, float armor = 0f, float growth = 1f, float luck = 1f
            )
        {
            this.maxHealth = maxHealth;
            this.recovery = recovery;
            this.moveSpeed = moveSpeed;
            this.might = might;
            this.projectileSpeed = projectileSpeed;
            this.magnet = magnet;
            this.area = area;
            this.armor = armor;
            this.growth = growth;
            this.luck = luck;
        }

        public static Stats operator +(Stats s1, Stats s2) // important to recognize this functionality
        {
            s1.maxHealth += s2.maxHealth;
            s1.recovery += s2.recovery;
            s1.moveSpeed += s2.moveSpeed;
            s1.might += s2.might;
            s1.projectileSpeed += s2.projectileSpeed;
            s1.magnet += s2.magnet;
            s1.area += s2.area;
            s1.armor += s2.armor;
            s1.growth += s2.growth;
            s1.luck += s2.luck;
            return s1;
        }
    }

    public Stats stats = new Stats(100f);
}
