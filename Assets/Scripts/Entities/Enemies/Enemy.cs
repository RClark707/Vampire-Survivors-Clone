using System.Collections;
using UnityEngine;

[RequireComponent(typeof(EnemyMovement))]
[RequireComponent(typeof(SpriteRenderer))]
public class Enemy : Entity
{
    EnemySpawner es;

    [Header("Enemy Stats")]
    public EnemyStats stats;
    [HideInInspector]
    public float movementSpeed { get; private set; }
    [HideInInspector]
    public float damage { get; private set; }

    [Header("Damage Feedback")]
    public Color damagedColor = new Color(1, 0, 0, 1);
    public float damageFlashDuration = 0.2f;
    public float deathFadeTime = 0.6f;
    Color originalColor;
    SpriteRenderer sr;
    EnemyMovement em;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Awake()
    {
        name = stats.name;
        health = stats.MaxHealth;
        movementSpeed = stats.MovementSpeed;
        damage = stats.Damage;

        es = FindAnyObjectByType<EnemySpawner>();
        sr = GetComponent<SpriteRenderer>();
        em = GetComponent<EnemyMovement>();

        originalColor = sr.color;

        base.Awake();
    }

    public override void TakeDamage(float amount)
    {
        // rewrite this function to take a source as a parameter
        StartCoroutine(DamageFlash());
        // em.ApplyKnockback();
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

    IEnumerator DamageFlash()
    {
        sr.color = damagedColor;
        yield return new WaitForSeconds(damageFlashDuration);
        sr.color = originalColor;
    }

    // IEnumerator KillFade()
    // {
    // 
    // }
}
