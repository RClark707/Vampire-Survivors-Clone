using UnityEngine;

/// <summary>
/// This GameObject will be spawned as an effect of a weapon using the Attack() method, e.g. projectiles, auras, pulses
/// </summary>
public abstract class WeaponEffect : MonoBehaviour // this class is meant to be subclassed in order to be attached to GOs
{
    [HideInInspector] public Player owner;
    [HideInInspector] public WeaponB weapon;

    public float GetDamage()
    {
        return weapon.GetDamage();
    }
}
