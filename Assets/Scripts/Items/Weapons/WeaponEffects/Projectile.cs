using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : WeaponEffect
{
    public enum KnockbackSource { Projectile, Owner };
    public KnockbackSource knockbackSource = KnockbackSource.Projectile;
    public bool hasAutoAim = false;
    public Vector3 rotationSpeed = new Vector3(0f, 0f, 0f);

    protected Rigidbody2D rb;
    protected int piercing;

    protected virtual void Start()
    {
        WeaponB.Stats stats = weapon.GetStats();

        rb = GetComponent<Rigidbody2D>();
        if (rb.bodyType == RigidbodyType2D.Dynamic)
        {
            rb.angularVelocity = rotationSpeed.z;
            rb.linearVelocity = transform.right * stats.speed;
        }

        float area = stats.area == 0 ? 1f : stats.area;
        transform.localScale = new Vector3(
            area = Mathf.Sign(transform.localScale.x),
            area = Mathf.Sign(transform.localScale.y),
            1f
            );

        piercing = stats.piercing;

        if (stats.lifespan > 0) Destroy(gameObject, stats.lifespan);

        if (hasAutoAim) AcquireAutoAimFacing();
        else
        {
            SetSpriteRotationFromPlayerDirection(owner.GetComponent<PlayerMovement>().lastMoveDirection);
        }
    }

    // automatically track a new target to launch towards
    public virtual void AcquireAutoAimFacing()
    {
        float aimAngle;

        Enemy[] targets = FindObjectsByType<Enemy>(FindObjectsSortMode.None);

        if (targets.Length > 0)
        {
            Enemy target = targets[Random.Range(0, targets.Length)];
            Vector2 difference = target.transform.position - transform.position;
            aimAngle = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        }
        else
        {
            aimAngle = Random.Range(0f, 360f);
        }

        transform.rotation = Quaternion.Euler(0f, 0f, aimAngle); // this assumes our projectile faces UP
    }

    /// <summary>
    /// This function sets the rotation of a sprite based on a given facing vector when spawned
    /// </summary>
    /// <param name="fixedDirection"></param>
    public virtual void SetSpriteRotationFromPlayerDirection(Vector3 fixedDirection, float offsetAngle = 90f)
    {
        float angle = Mathf.Atan2(fixedDirection.y, fixedDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - offsetAngle);
    }

    protected virtual void FixedUpdate()
    {
        if (rb.bodyType == RigidbodyType2D.Kinematic)
        {
            WeaponB.Stats stats = weapon.GetStats();
            transform.position += transform.right * stats.speed * Time.fixedDeltaTime;
            rb.MovePosition(transform.position);
            transform.Rotate(rotationSpeed * Time.fixedDeltaTime);
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        Enemy e = other.GetComponent<Enemy>();
        DestructibleProp dp = other.GetComponent<DestructibleProp>();

        if (e)
        {
            // should we use projectile knockback or owner knockback?
            Vector3 source = knockbackSource == KnockbackSource.Owner && owner ? owner.transform.position : transform.position;

            e.TakeDamage(GetDamage(), source);

            // Debug.Log($"The {name} just hit {e.name} for {GetDamage()} damage.");

            WeaponB.Stats stats = weapon.GetStats();
            piercing--;
            if (stats.hitEffect)
            {
                Destroy(Instantiate(stats.hitEffect, transform.position, Quaternion.identity), 5f); // this is a static hit effect lifespan
            }
        }
        else if (dp)
        {
            dp.TakeDamage(GetDamage(), new Vector3(0, 0, 0)); // this source isn't real, get rid of it!
            piercing--;

            WeaponB.Stats stats = weapon.GetStats();
            if (stats.hitEffect)
            {
                Destroy(Instantiate(stats.hitEffect, transform.position, Quaternion.identity), 5f); // this is a static hit effect lifespan
            }
        }

        if (piercing <= 0f) Destroy(gameObject);
    }
}
