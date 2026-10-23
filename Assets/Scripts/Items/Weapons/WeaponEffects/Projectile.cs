using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : WeaponEffect
{
    public enum KnockbackSource { Projectile, Owner };
    public KnockbackSource knockbackSource = KnockbackSource.Projectile;
    public enum SpriteDirection { Left, Right, Up, Down };
    [Tooltip("Which direction is \"forward\" for your sprite?")]
    public SpriteDirection spriteDirection = SpriteDirection.Up;
    public bool hasAutoAim = false;
    [Tooltip("Typically, only z-axis rotation will have an effect. May affect Physics.")]
    public Vector3 rotationSpeed = new Vector3(0f, 0f, 0f);
    [Tooltip("This is the base speed of the projectile, modified by the percentile Projectile Speed stats.")]
    public float baseSpeed = 5f;

    protected Rigidbody2D rb;
    protected int piercing;
    protected float area;

    protected virtual void Start()
    {
        transform.SetParent(GameObject.Find("ProjectileParent").transform);

        Weapon.Stats stats = weapon.GetStats();

        if (hasAutoAim)
        {
            SetAutoAimAngle();
        }
        else
        {
            SetSpriteRotationFromPlayerDirection(owner.GetComponent<PlayerMovement>().lastMoveDirection);
        }

        rb = GetComponent<Rigidbody2D>();
        if (rb.bodyType == RigidbodyType2D.Dynamic)
        {
            rb.angularVelocity = rotationSpeed.z;

            Vector3 forward;

            switch (spriteDirection)
            {
                case SpriteDirection.Left:
                    forward = -transform.right;
                    break;
                default:
                case SpriteDirection.Right:
                    forward = transform.right;
                    break;
                case SpriteDirection.Up:
                    forward = transform.up;
                    break;
                case SpriteDirection.Down:
                    forward = -transform.up;
                    break;
            }

            rb.linearVelocity = forward * baseSpeed * weapon.GetProjectileSpeedPercentage();
            // + (Vector3)owner.GetComponent<Rigidbody2D>().linearVelocity;
            // Debug.Log($"The {stats.name} spawned a projectile with {rb.linearVelocity} linear velocity.");
            // Debug.Log($"The {stats.name} spawned a projectile with {rb.angularVelocity} angular velocity.");
        }

        area = weapon.GetAreaPercentage();
        if (area <= 0f) area = 1f;
        transform.localScale = new Vector3(
            area = Mathf.Sign(transform.localScale.x),
            area = Mathf.Sign(transform.localScale.y),
            1f
            );

        piercing = stats.piercing;

        if (weapon.GetDuration() > 0) Destroy(gameObject, weapon.GetDuration());
    }

    // automatically track a new target to launch towards
    public virtual void SetAutoAimAngle()
    {
        float aimAngle;

        if (weapon.nearestEnemy)
        {
            // Debug.Log($"Nearest Enemy: {weapon.nearestEnemy.transform.position}");
            Vector2 difference = weapon.nearestEnemy.transform.position - transform.position;
            aimAngle = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        }
        else
        {
            aimAngle = Random.Range(0f, 360f);
        }

        // This line rotates our projectile so that linearVelocity sends it in the aimAngle direction
        // This assumes our projectile sprite faces UP

        switch (spriteDirection)
        {
            case SpriteDirection.Left:
                aimAngle -= 180f;
                break;
            default:
            case SpriteDirection.Right:
                break;
            case SpriteDirection.Up:
                aimAngle -= 90f;
                break;
            case SpriteDirection.Down:
                aimAngle += 90f;
                break;
        }

        transform.rotation = Quaternion.Euler(0f, 0f, aimAngle);
        // Debug.Log($"The {name} is launching at an {aimAngle} degree angle towards {weapon.nearestEnemy}.");
        // Debug.Log($"Projectile Rotation: {transform.rotation.eulerAngles}");
    }

    public virtual void SetSpriteRotationFromPlayerDirection(Vector2 fixedDirection, float offsetAngle = 90f)
    {
        // TODO: Check if this needs to be updated with Sprite Direction switch
        // We don't want this to run for weapons that change their angles on their own
        float angle = Mathf.Atan2(fixedDirection.y, fixedDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - offsetAngle);
        // Debug.Log($"The {name} is launching at a fixed {angle - offsetAngle} degree angle.");
    }

    protected virtual void FixedUpdate()
    {
        if (rb.bodyType == RigidbodyType2D.Kinematic)
        {
            Weapon.Stats stats = weapon.GetStats();
            // TODO: Check if we should use GetProjectileSpeed()
            transform.position += transform.right * weapon.Owner.ActualStats.projectileSpeed * Time.fixedDeltaTime;
            rb.MovePosition(transform.position);
            transform.Rotate(rotationSpeed * Time.fixedDeltaTime);
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        Enemy e = other.GetComponent<Enemy>();
        DestructibleProp dp = other.GetComponent<DestructibleProp>();

        if (e && !e.dead)
        {
            // should we use projectile knockback or owner knockback?
            Vector3 source = knockbackSource == KnockbackSource.Owner && owner ? owner.transform.position : transform.position;

            e.TakeDamage(GetDamage(), source); // this means projectiles don't apply their own knockback!
            weapon.ApplyBuffs(e);
            piercing--;

            Weapon.Stats stats = weapon.GetStats();
            if (stats.hitEffect)
            {
                Destroy(Instantiate(stats.hitEffect, transform.position, Quaternion.identity), 5f); // this is a static hit effect lifespan
            }

            // Debug.Log($"The {name} just hit {e.name} for {GetDamage()} damage.");
        }
        else if (dp)
        {
            // dp.TakeDamage(GetDamage()); // this source isn't real, get rid of it!
            piercing--;

            Weapon.Stats stats = weapon.GetStats();
            if (stats.hitEffect)
            {
                Destroy(Instantiate(stats.hitEffect, transform.position, Quaternion.identity), 5f); // this is a static hit effect lifespan
            }
        }

        if (piercing <= 0f) Destroy(gameObject);
    }
}
