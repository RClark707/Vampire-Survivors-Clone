using System.Collections.Generic;
using UnityEngine;

public class Aura : WeaponEffect
{
    Dictionary<Enemy, float> affectedTargets = new Dictionary<Enemy, float>();
    List<Enemy> targetsToUnaffect = new List<Enemy>();

    void Update()
    {
        Dictionary<Enemy, float> affectedTargetsCopy = new Dictionary<Enemy, float>(affectedTargets);

        foreach (KeyValuePair<Enemy, float> pair in affectedTargetsCopy)
        {
            affectedTargets[pair.Key] -= Time.deltaTime;
            if (pair.Value <= 0)
            {
                if (targetsToUnaffect.Contains(pair.Key))
                {
                    affectedTargets.Remove(pair.Key);
                    targetsToUnaffect.Remove(pair.Key);
                }
                else
                {
                    WeaponB.Stats stats = weapon.GetStats();
                    affectedTargets[pair.Key] = stats.cooldown;
                    pair.Key.TakeDamage(GetDamage(), transform.position); // also add a stats.knockback argument?
                    // player a hit effect if it is assigned

                    if (stats.hitEffect != null)
                    {
                        Destroy(Instantiate(stats.hitEffect, pair.Key.transform.position, Quaternion.identity), 5f);
                    }
                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Enemy e))
        {
            if (!affectedTargets.ContainsKey(e))
            {
                affectedTargets.Add(e, 0f);
            }
            else
            {
                if (targetsToUnaffect.Contains(e))
                {
                    targetsToUnaffect.Remove(e);
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent(out Enemy e))
        {
            if (affectedTargets.ContainsKey(e))
            {
                targetsToUnaffect.Add(e);
            }
        }
    }
}
