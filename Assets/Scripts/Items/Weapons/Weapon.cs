using UnityEngine;

public abstract class Weapon : Item // because this is an abstract class, we need to subclass it in order to attach to GOs
{
    [System.Serializable]
    public class Stats : LevelData
    {
        [Header("Visuals")]
        [Tooltip("The object to instantiate as a projectile.")]
        public Projectile projectilePrefab;
        [Tooltip("The object to instantiate as an aura.")]
        public Aura auraPrefab;
        [Tooltip("The VFX to instantiate when the weapon damages an enemy, and when the weapon is used.")]
        public ParticleSystem hitEffect, procEffect; // hit effects are played on the enemy, procs are played on the player
        [Tooltip("The SFX to play when the when is used to attack.")]
        public AudioClip soundEffect;
        [Tooltip("What random values to apply to the spawn position of the weapon's projectile or aura.")]
        public Rect spawnVariance;

        [Header("Values")]
        [Tooltip("After how many seconds should we destroy any instantiated projectiles.")]
        public float duration;
        [Tooltip("The stats of the weapon, leave at 0 if you want to ignore it.")]
        public float damage, damageVariance, area, projectileSpeed, cooldown, projectileInterval, knockback;
        public int amount, piercing, maxInstances;

        public Entity.BuffInfo[] appliedBuffs;

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
                duration = s1.duration + s2.duration,
                damage = s1.damage + s2.damage,
                damageVariance = s1.damageVariance + s2.damageVariance,
                area = s1.area + s2.area,
                projectileSpeed = s1.projectileSpeed + s2.projectileSpeed,
                cooldown = s1.cooldown + s2.cooldown,
                amount = s1.amount + s2.amount,
                piercing = s1.piercing + s2.piercing,
                projectileInterval = s1.projectileInterval + s2.projectileInterval,
                knockback = s1.knockback + s2.knockback,
                appliedBuffs = s2.appliedBuffs == null || s2.appliedBuffs.Length <= 0 ? s1.appliedBuffs : s2.appliedBuffs
            };

            return result;
        }

        public float GetDamageWithVariance()
        {
            return damage + Random.Range(0, damageVariance);
        }
    }

    protected PlayerMovement playerMovement;
    protected Stats currentStats;
    protected float currentCooldown;
    public Enemy nearestEnemy;
    protected float nearestEnemyDistance;

    // some weapons need to be initialized
    public virtual void Initialize(WeaponStats stats)
    {
        // Debug.Log($"{name} has been initialized.");
        base.Initialize(stats);
        playerMovement = owner.GetComponent<PlayerMovement>();

        this.statsData = stats;
        currentStats = stats.baseStats;
        ResetWeaponCooldown();
    }

    protected virtual void Update()
    {
        FindNearestEnemy();
        currentCooldown -= Time.deltaTime;
        if (currentCooldown <= 0)
        {
            Attack(currentStats.amount + Owner.ActualStats.amount);
        }
    }

    public override bool LevelUp(bool updateUI = true)
    {
        base.LevelUp(updateUI);

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
            ResetWeaponCooldown();
            return true;
        }
        return false;
    }

    // This returns the flat duration of the weapon
    public virtual float GetDuration()
    {
        return currentStats.duration + Owner.ActualStats.duration;
    }

    // This returns the flat cooldown of the weapon, after percentage modification
    public virtual float GetCooldown()
    {
        return currentStats.cooldown * Owner.ActualStats.cooldown;
    }

    // this gets the flat amount of damage for the weapon, factoring in damage variance and percentage might
    public virtual float GetDamage()
    {
        return currentStats.GetDamageWithVariance() * Owner.ActualStats.might;
    }

    // This returns a percentage with which to multiply by (or use in place of 1)
    public virtual float GetAreaPercentage()
    {
        return currentStats.area + Owner.ActualStats.area;
    }

    // This returns a percentage with which to multiply by
    public virtual float GetProjectileSpeedPercentage()
    {
        return currentStats.projectileSpeed + Owner.ActualStats.projectileSpeed;
    }

    // This returns the flat amount of projectiles for the weapon
    public virtual int GetAmount()
    {
        return currentStats.amount + Owner.ActualStats.amount;
    }

    public virtual Stats GetStats()
    {
        return currentStats;
    }

    public virtual bool ResetWeaponCooldown(bool strict = false)
    {
        if (strict && currentCooldown > 0) return false;
        float actualCooldown = GetCooldown(); // this will make the cooldown longer if our cooldown is above 1
        currentCooldown = Mathf.Min(actualCooldown, currentCooldown + actualCooldown);
        return true;
    }

    protected virtual void FindNearestEnemy()
    {
        Enemy[] allEnemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);

        float thisDistance;
        foreach (Enemy enemy in allEnemies)
        {
            if (nearestEnemy == null)
            {
                nearestEnemy = enemy;
                nearestEnemyDistance = (owner.transform.position - nearestEnemy.transform.position).magnitude;
            }
            else
            {
                thisDistance = (owner.transform.position - enemy.transform.position).magnitude;
                if (thisDistance < nearestEnemyDistance)
                {
                    nearestEnemy = enemy;
                    nearestEnemyDistance = thisDistance;
                }
            }
        }
    }

    public void ApplyBuffs(Entity e)
    {
        foreach (Entity.BuffInfo b in GetStats().appliedBuffs)
        {
            e.ApplyBuff(b, owner.ActualStats.duration);
        }
    }
}
