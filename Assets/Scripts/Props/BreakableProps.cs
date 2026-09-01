using UnityEngine;

public class BreakableProps : MonoBehaviour
{
    public PropStats propStats;
    float health;

    void Awake()
    {
        name = propStats.name;
        health = propStats.MaxHealth;
    }

    public void TakeDamage(float dmg)
    {
        health -= dmg;

        Debug.Log($"{name} has {health} health left!");

        if (health <= 0)
        {
            // Destroy the prop
            Kill();
        }
    }

    public void Kill()
    {
        Debug.Log($"The {name} was destroyed.");
        Destroy(gameObject);
    }
}
