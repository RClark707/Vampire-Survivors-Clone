using UnityEngine;

public class Enemy : MonoBehaviour
{
    public EnemyStats enemyStats;

    // These are the Enemy's Current Stat Values, NOT the base stats
    float health;
    float movementSpeed;
    float damage;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        name = enemyStats.name;
        health = enemyStats.MaxHealth;
        movementSpeed = enemyStats.MovementSpeed;
        damage = enemyStats.Damage;
    }

    public bool TakeDamage(float dmg)
    {
        health -= dmg;
        Debug.Log($"{name} has {health} health left!");

        if (health <= 0)
        {
            Debug.Log($"The {name} is dead!");
            Kill();
            return true;
        }

        return false;
    }

    public void Kill()
    {
        Destroy(gameObject);
    }
}
