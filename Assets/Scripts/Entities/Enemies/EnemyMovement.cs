using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class EnemyMovement : MonoBehaviour
{
    Enemy enemy;
    Transform playerTransform;

    // Knockback
    Vector2 knockbackVelocity;
    float knockbackDuration;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerTransform = FindAnyObjectByType<PlayerMovement>().transform;
        enemy = GetComponent<Enemy>();
    }

    // Update is called once per frame
    void Update()
    {
        if (knockbackDuration > 0)
        {
            transform.position += (Vector3)knockbackVelocity * Time.deltaTime;
            knockbackDuration -= Time.deltaTime;
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, enemy.movementSpeed * Time.deltaTime);
        }

        CheckEnemyPlayerDistance();
    }

    void CheckEnemyPlayerDistance()
    {
        float separation = (playerTransform.position - transform.position).magnitude;

        if (separation >= 50f) // is the enemy too far away to worry about?
        {
            Destroy(gameObject);
        }

        // we can check here to see if this is the nearest enemy to the player too
    }

    // this function is meant to be called elsewhere
    public void ApplyKnockback(Vector2 velocity, float duration)
    {
        if (knockbackDuration > 0) return;

        knockbackVelocity = velocity;
        knockbackDuration = duration;
    }
}
