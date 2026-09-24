using UnityEngine;

public abstract class WeaponB : ItemB // because this is an abstract class, we need to subclass it in order to attach to GOs
{
    [System.Serializable]
    public struct Stats
    {
        public string name, description;

        [Header("Visuals")]
        public Projectile projectilePrefab;
        public Aura auraPrefab;
        public ParticleSystem hitEffect;
        public Rect spawnVariance;

        [Header("Values")]
        public float lifespan;
        public float damage, damageVariance, area, speed, cooldown, projectileInterval, knockback;
        public int number, piercing, maxInstances;

        // define addition between two Stats - this is Abstract Algebra!
        public static Stats operator +(Stats s1, Stats s2)
        {
            Stats result = new Stats
            {
                name = s2.name ?? s1.name,
                description = s2.description ?? s1.description,
                projectilePrefab = s2.projectilePrefab ?? s1.projectilePrefab,
                auraPrefab = s2.auraPrefab ?? s1.auraPrefab,
                hitEffect = s2.hitEffect ?? s1.hitEffect,
                spawnVariance = s2.spawnVariance,
                lifespan = s1.lifespan + s2.lifespan,
                damage = s1.damage + s2.damage,
                damageVariance = s1.damageVariance + s2.damageVariance,
                area = s1.area + s2.area,
                speed = s1.speed + s2.speed,
                cooldown = s1.cooldown + s2.cooldown,
                number = s1.number + s2.number,
                piercing = s1.piercing + s2.piercing,
                projectileInterval = s1.projectileInterval + s2.projectileInterval,
                knockback = s1.knockback + s2.knockback
            };

            return result;
        }

        public float GetDamage()
        {
            return damage + Random.Range(0, damageVariance);
        }
    }

    protected PlayerMovement pm;
    protected Stats currentStats;
    public WeaponStatsB statsData;
    protected float currentCooldown;

    // some weapons need to be initialized
    public virtual void Initialize(WeaponStatsB stats)
    {
        base.Initialize(stats);
        pm = owner.GetComponent<PlayerMovement>();

        this.statsData = stats;
        currentStats = stats.baseStats;
        currentCooldown = currentStats.cooldown;
    }

    protected virtual void Awake()
    {
        if (statsData) currentStats = statsData.baseStats;
    }

    protected virtual void Start()
    {
        if (statsData) Initialize(statsData);
    }

    protected virtual void Update()
    {
        currentCooldown -= Time.deltaTime;
        if (currentCooldown <= 0)
        {
            Attack(currentStats.number);
        }
    }

    public override bool LevelUp()
    {
        // base.LevelUp(); // this does nothing but return true
        if (!CanLevelUp())
        {
            Debug.LogWarning($"Cannot level up your {name} to level {currentLevel + 1}. It has already reached the max level of {maxLevel}");
            return false;
        }

        currentStats += statsData.GetLevelData(++currentLevel);
        return true;
    }

    public virtual bool CanAttack()
    {
        return currentCooldown <= 0;
    }

    // this method is meant to be overriden, but called from base
    // TODO: Make all weapons scale with area
    protected virtual bool Attack(int attackAmount)
    {
        if (CanAttack())
        {
            currentCooldown += currentStats.cooldown;
            return true;
        }
        return false;
    }

    // this gets the amount of damage for the weapon, factoring in damage variance and might
    public virtual float GetDamage()
    {
        return currentStats.GetDamage() * owner.Might;
    }

    public virtual Stats GetStats()
    {
        return currentStats;
    }
}
