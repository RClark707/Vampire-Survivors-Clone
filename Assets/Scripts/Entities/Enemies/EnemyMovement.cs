using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class EnemyMovement : Sortable
{
    protected Enemy enemy;
    protected Player player;
    Rigidbody2D rb;

    protected bool knockedBack;
    protected Vector2 knockbackDirection;
    protected float knockbackDuration;
    [System.Flags]
    public enum KnockbackBehavior { Duration = 1, Velocity = 2 }
    [Header("Knockback")]
    public KnockbackBehavior knockbackBehavior = KnockbackBehavior.Velocity;

    [Header("Distance From Player")]
    public OutOfFrameBehavior outOfFrameAction = OutOfFrameBehavior.RespawnAtEdge;
    public enum OutOfFrameBehavior { None, RespawnAtEdge, Despawn }

    protected bool spawnedOutOfFrame = false;

    protected override void Start()
    {
        base.Start();

        spawnedOutOfFrame = !SpawnController.IsWithinBoundaries(transform);
        enemy = GetComponent<Enemy>();
        rb = GetComponent<Rigidbody2D>();

        Player[] allPlayers = FindObjectsByType<Player>(FindObjectsSortMode.None);
        player = allPlayers[Random.Range(0, allPlayers.Length)];
    }

    protected virtual void Update()
    {
        if (knockbackDuration > 0f)
        {
            transform.position += (Vector3)knockbackDirection * Time.deltaTime;
            knockbackDuration -= Time.deltaTime;
        }
        else
        {
            Move();
            HandleEnemyOutOfFrame();
        }
    }

    public virtual void Move()
    {
        if (rb)
        {
            Vector2 position = Vector2.MoveTowards( // this is intended for use with kinematic bodies
                rb.position,
                player.transform.position,
                enemy.ActualStats.moveSpeed * Time.deltaTime
                );
            rb.MovePosition(position);
            Vector3 adjustedPos = transform.position;
            adjustedPos.z = 0f;
            transform.position = adjustedPos;
        }
        else // no rigidbody
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                player.transform.position,
                enemy.ActualStats.moveSpeed * Time.deltaTime
                );
        }
    }

    // This function gets called by Enemy.TakeDamage()
    public virtual void Knockback(Vector2 velocity, float duration)
    {
        if (knockbackDuration > 0f) return;

        if (knockbackBehavior == 0) return;

        float power = 1f;
        bool reducesVelocity = (knockbackBehavior & KnockbackBehavior.Velocity) > 0f,
            reducesDuration = (knockbackBehavior & KnockbackBehavior.Duration) > 0f;

        if (reducesVelocity && reducesDuration) power = 0.5f;

        knockbackDirection = velocity * (reducesVelocity ? Mathf.Pow(enemy.ActualStats.knockbackMultiplier, power) : 1);
        knockbackDuration = duration * (reducesDuration ? Mathf.Pow(enemy.ActualStats.knockbackMultiplier, power) : 1);
    }

    protected virtual void HandleEnemyOutOfFrame()
    {
        if (!SpawnController.IsWithinBoundaries(transform)) // are we not on the screen
        {
            switch (outOfFrameAction)
            {
                case OutOfFrameBehavior.None: default: break;
                case OutOfFrameBehavior.RespawnAtEdge:
                    transform.position = SpawnController.GenerateSpawnPosition();
                    break;
                case OutOfFrameBehavior.Despawn:
                    if (!spawnedOutOfFrame) // if we spawned out of frame, don't worry about it
                    {
                        Destroy(gameObject);
                    }
                    break;
            }
        }
        else // we are within the screen boundaries
        {
            spawnedOutOfFrame = false;
        }
    }
}