using UnityEngine;

public class AxeWeapon : ProjectileWeaponB
{
    protected override float GetSpawnAngle()
    {
        int offset = currentAttackCount > 0 ? currentStats.number - currentAttackCount : 0;
        return 90f - Mathf.Sign(pm.lastMoveDirection.x) * (5 * offset); // offset each axe by 5 degrees from the original 90 degrees
        // this is bugged out, when facing left it works mostly OK (if we were facing right)

    }

    protected override Vector2 GetSpawnOffset(float spawnAngle = 0)
    {
        return new Vector2(
            Random.Range(currentStats.spawnVariance.xMin, currentStats.spawnVariance.xMax),
            Random.Range(currentStats.spawnVariance.yMin, currentStats.spawnVariance.yMax)
            );
    }
}
