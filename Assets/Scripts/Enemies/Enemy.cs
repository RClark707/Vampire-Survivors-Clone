using UnityEngine;

public class Enemy : MonoBehaviour
{
    // These are the Enemy's Current Stat Values, NOT the base stats
    [Header("Enemy Stats")]
    public EnemyStats enemyStats;
    [HideInInspector]
    public float health { get; private set; }
    [HideInInspector]
    public float movementSpeed { get; private set; }
    [HideInInspector]
    public float damage { get; private set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        name = enemyStats.name;
        health = enemyStats.MaxHealth;
        movementSpeed = enemyStats.MovementSpeed;
        damage = enemyStats.Damage;
    }

    public void TakeDamage(float dmg)
    {
        health -= dmg;
        Debug.Log($"{name} has {health} health left!");

        if (health <= 0)
        {
            Debug.Log($"The {name} is dead!");
            Kill();
        }
    }

    public void Kill()
    {
        Destroy(gameObject);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (collision.TryGetComponent(out Player player))
            {
                player.TakeDamage(damage);
            }
        }
    }
}
