using UnityEngine;

public abstract class WeaponB : ItemB // because this is an abstract class, we need to subclass it in order to attach to GOs
{
    [System.Serializable]
    public class Stats : LevelData
    {
        [Header("Visuals")]
        public Projectile projectilePrefab;
        public Aura auraPrefab;
        public ParticleSystem hitEffect, procEffect; // hit effects are played on the enemy, procs are played on the player
        public Rect spawnVariance;

        [Header("Values")]
        public float lifespan;
        public float damage, damageVariance, area, speed, cooldown, projectileInterval, knockback;
        public int number, piercing, maxInstances;

        public EntityB.BuffInfo[] appliedBuffs;

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
                knockback = s1.knockback + s2.knockback,
                appliedBuffs = s2.appliedBuffs == null || s2.appliedBuffs.Length <= 0 ? s1.appliedBuffs : s2.appliedBuffs
            };

            return result;
        }

        public float GetDamage()
        {
            return damage + Random.Range(0, damageVariance);
        }
    }

    protected PlayerMovement playerMovement;
    protected Stats currentStats;
    protected float currentCooldown;

    // some weapons need to be initialized
    public virtual void Initialize(WeaponStatsB stats)
    {
        Debug.Log($"{name} has been initialized.");
        base.Initialize(stats);
        playerMovement = owner.GetComponent<PlayerMovement>();

        this.statsData = stats;
        currentStats = stats.baseStats;
        ActivateCooldown();
    }

    protected virtual void Update()
    {
        currentCooldown -= Time.deltaTime;
        if (currentCooldown <= 0)
        {
            Attack(currentStats.number + Owner.ActualStats.amount);
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

        // worth telling the game to ActivateCooldown(); here

        currentStats += (Stats)statsData.GetLevelData(++currentLevel);
        return true;
    }

    public virtual bool CanAttack()
    {
        if (Mathf.Approximately(0f, owner.ActualStats.might)) return false; // this is a preference thing, maybe
        return currentCooldown <= 0;
    }

    // this method is meant to be overriden, but called from base
    protected virtual bool Attack(int attackAmount)
    {
        if (CanAttack())
        {
            ActivateCooldown();
            return true;
        }
        return false;
    }

    // this gets the amount of damage for the weapon, factoring in damage variance and might
    public virtual float GetDamage()
    {
        return currentStats.GetDamage() * Owner.ActualStats.might;
    }

    public virtual float GetArea()
    {
        return currentStats.area + Owner.ActualStats.area;
    }

    public virtual Stats GetStats()
    {
        return currentStats;
    }

    public virtual bool ActivateCooldown(bool strict = false)
    {
        if (strict && currentCooldown > 0) return false;
        float actualCooldown = currentStats.cooldown * Owner.ActualStats.cooldown; // this will make the cooldown longer if our cooldown is above 1
        currentCooldown = Mathf.Min(actualCooldown, currentCooldown + actualCooldown);
        return true;
    }

    public void ApplyBuffs(EntityB e)
    {
        foreach (EntityB.BuffInfo b in GetStats().appliedBuffs)
        {
            e.ApplyBuff(b, owner.ActualStats.duration);
        }
    }
}
