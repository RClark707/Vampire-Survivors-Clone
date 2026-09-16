using System.Collections;
using UnityEngine;

[RequireComponent(typeof(EnemyMovement))]
public class Enemy : Entity
{
    EnemySpawner es;
    bool isElite;
    public bool IsElite
    {
        get { return isElite; }
        set
        {
            isElite = value;
            if (isElite)
            {
                // set the sprite to be larger
                // modify the stats by 2 times
                // apply a shader
            }
        }
    }

    [Header("Enemy Stats")]
    public EnemyStats stats;
    public float movementSpeed { get; private set; }
    public float damage { get; private set; }

    [Header("Damage Feedback")]
    EnemyMovement em;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Awake()
    {
        name = stats.name;
        health = stats.MaxHealth;
        movementSpeed = stats.MovementSpeed;
        damage = stats.Damage;

        es = FindAnyObjectByType<EnemySpawner>();
        em = GetComponent<EnemyMovement>();

        base.Awake();
    }

    public override void TakeDamage(float amount)
    {
        base.TakeDamage(amount);
    }

    public void ApplyKnockback(Vector2 source, float amount)
    {
        em.knockedBack = true;
        em.knockback = (source - (Vector2)transform.position).normalized * amount;
    }

    public override void Kill()
    {
        Debug.Log($"The {name} has been killed.");
        StartCoroutine(KillFade());
        // base.Kill();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && collision.TryGetComponent(out Player player))
        {
            player.TakeDamage(damage);
        }
    }

    IEnumerator KillFade()
    {
        WaitForEndOfFrame w = new WaitForEndOfFrame();
        float t = 0, origAlpha = sr.color.a;

        while (t < deathFadeTime)
        {
            yield return w;
            t += Time.deltaTime;

            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, (1 - t / deathFadeTime) * origAlpha); // linear interpolate the alpha
        }

        Destroy(gameObject);
        es.OnEnemyKilled();
        if (pc) pc.OnHostKilled(); // update this with reference to if we are an elite enemy!
    }
}
