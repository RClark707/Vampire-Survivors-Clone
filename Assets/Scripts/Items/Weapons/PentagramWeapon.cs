using System.Collections;
using UnityEngine;

public class PentagramWeapon : Weapon
{
    public override bool LevelUp(bool updateUI = true)
    {
        if (!base.LevelUp(updateUI)) return false;

        OnEquip(); // this replaces setting the transform scale

        return true;
    }

    public override void OnEquip()
    {
        Attack(0);
    }

    public override bool CanAttack()
    {
        // we can always use the Pentagram regardless of might
        return currentCooldown <= 0;
    }

    /// <summary>
    /// The overridden Attack function's purpose is to call an enumerator to control the screen VFX.
    /// The single parameter is left unused.
    /// </summary>
    /// <param name="attackAmount"></param>
    /// <returns></returns>
    protected override bool Attack(int attackAmount)
    {
        if (!CanAttack()) return false;

        StartCoroutine(CreateGlyph(GetDuration()));

        ResetWeaponCooldown();

        return true;
    }

    protected void ScreenWipe()
    {
        // Find all enemies
        Enemy[] allEnemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);

        foreach (Enemy enemy in allEnemies)
        {
            // Check if the enemy is on the screen
            Renderer r = enemy.GetComponent<Renderer>();
            if (!r || !r.isVisible)
            {
                continue;
            }

            // Deal damage equal to the enemy's max health, or 660, whichever is higher
            float damageAmount = enemy.ActualStats.maxHealth > 660 ? enemy.ActualStats.maxHealth : Random.Range(655, 665);
            enemy.TakeDamage(damageAmount, owner.transform.position);
        }

        // Decide if items will be erased

        // Find all items

        // Erase all items on the screen
    }

    protected IEnumerator CreateGlyph(float duration)
    {
        // Play the sound effect
        if (currentStats.soundEffect)
        {
            // implement weapon sound effects
        }

        if (currentStats.procEffect)
        {
            Destroy(Instantiate(currentStats.procEffect, owner.transform), 5f);
        }

        yield return new WaitForSeconds(5f);

        ScreenWipe();
    }
}
