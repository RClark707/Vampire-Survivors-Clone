using UnityEngine;

public class Entity : MonoBehaviour
{
    [HideInInspector]
    public float health { get; set; }
    PickupController pc;

    public virtual void Awake()
    {
        if (TryGetComponent(out PickupController component))
        {
            pc = component;
        }
    }

    public virtual void TakeDamage(float amount)
    {
        health = Mathf.Max(health - amount, 0f);
        Debug.Log($"The {name} has {health} health left after taking {amount} damage!");
        if (health <= 0f)
        {
            Kill();
        }
    }

    public virtual void Kill()
    {
        Debug.Log($"The {name} has been killed.");
        if (pc) pc.OnHostKilled();
        Destroy(gameObject);
    }
}
