using UnityEngine;

public class Pickup : MonoBehaviour, IPickup
{
    public PickupStats pickupStats;
    protected float strength;
    protected float speed;
    protected bool following;
    protected Transform targetPlayer;

    public virtual void Awake()
    {
        name = pickupStats.name;
        strength = pickupStats.PickupStrength;
        speed = pickupStats.MovementSpeed;
    }

    public virtual void Update()
    {
        if (following && targetPlayer)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPlayer.position, speed * Time.deltaTime);
        }
    }

    public virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Player player))
        {
            Debug.Log($"You picked up a {name}.");
            Collect(player);
        }
    }

    public virtual void Follow(Transform target)
    {
        following = true;
        targetPlayer = target;
        Debug.Log($"The {name} is now following {targetPlayer.name}");
    }

    // this method is meant to be overridden
    public virtual void Collect(Player player)
    {
        Destroy(gameObject);
    }
}
