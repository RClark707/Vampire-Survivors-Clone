using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class EnemyMovement : MonoBehaviour
{
    Enemy enemy;
    Transform playerTransform;

    [Header("Knockback")]
    public bool knockedBack;
    public Vector2 knockback;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerTransform = FindAnyObjectByType<PlayerMovement>().transform;
        enemy = GetComponent<Enemy>();
    }

    // Update is called once per frame
    void Update()
    {
        //if (knockbackDuration > 0)
        //{
        //    transform.position += (Vector3)knockbackVelocity * Time.deltaTime;
        //    knockbackDuration -= Time.deltaTime;
        //}
        //else
        //{
        //    transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, enemy.movementSpeed * Time.deltaTime);
        //}
        UpdatePositionWithKnockback();
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

    public void UpdatePositionWithKnockback()
    {
        if (knockedBack)
        {
            transform.position -= (Vector3)knockback * Time.deltaTime;
            knockback = Vector2.MoveTowards(knockback, Vector2.zero, 1f); // knockback is actually set by the Enemy script
            if (knockback == Vector2.zero)
            {
                knockedBack = false;
            }
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, enemy.movementSpeed * Time.deltaTime);
        }
    }
}
