using UnityEngine;

public class Enemy : Entity
{
    [Header("Enemy Stats")]
    public EnemyStats stats;
    [HideInInspector]
    public float movementSpeed { get; private set; }
    [HideInInspector]
    public float damage { get; private set; }

    [Header("Additional Variables")]
    EnemySpawner es;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Awake()
    {
        name = stats.name;
        health = stats.MaxHealth;
        movementSpeed = stats.MovementSpeed;
        damage = stats.Damage;

        es = FindAnyObjectByType<EnemySpawner>();
        base.Awake();
    }

    public override void TakeDamage(float amount)
    {
        base.TakeDamage(amount);
    }

    public override void Kill()
    {
        es.OnEnemyKilled();
        base.Kill();
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
