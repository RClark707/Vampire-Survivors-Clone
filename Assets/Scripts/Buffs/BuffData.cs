using UnityEngine;

[CreateAssetMenu(fileName = "New Buff", menuName = "Buff")]
public class BuffData : ScriptableObject
{
    public new string name = "New Buff";
    public Sprite icon;

    [System.Flags]
    public enum Type : byte { Buff = 1, Debuff = 2, Freeze = 4, Strong = 8 }
    public Type type;

    public enum BuffStackBehavior : byte { RefreshDurationOnly, StacksFully, DoesNotStack }
    public enum ModifierType : byte { Additive, Multiplicative }

    [System.Serializable]
    public class Stats
    {
        public string name;

        [Header("Visuals")]
        [Tooltip("Effect that is attached to the Game Object with the buff.")]
        public ParticleSystem effect;
        [Tooltip("The tint color of sprites affected by this buff.")]
        public Color tint = new Color(0, 0, 0, 1);
        [Tooltip("Whether this buff slows down or speeds up the animation of the affected Game Object.")]
        [Min(0f)] public float animationSpeed = 1f;

        [Header("Stats")]
        [Min(0f)] public float duration;
        [Min(0f)] public float damagePerSecond, healthPerSecond;

        [Tooltip("Controls how frequently the damage / heal per second applies.")]
        public float tickInterval = 0.25f;

        public BuffStackBehavior stackBehavior;
        public ModifierType modifierType;

        public Stats()
        {
            duration = 10f;
            damagePerSecond = 0f;
            healthPerSecond = 0f;
            tickInterval = 0.25f;
        }

        public CharacterStatsB.Stats playerModifier; // need a default for these that sets them all to 1
        public Enemy.Stats enemyModifier;
    }

    public Stats[] variations = new Stats[1]
    {
        new Stats {name = "Level 1"}
    };

    public float GetTickDamage(int variant = 0)
    {
        Stats s = Get(variant);
        return s.damagePerSecond * s.tickInterval;
    }

    public float GetTickHeal(int variant = 0)
    {
        Stats s = Get(variant);
        return s.healthPerSecond * s.tickInterval;
    }

    public Stats Get(int variant = -1)
    {
        return variations[Mathf.Max(0, variant)];
    }
}
