using UnityEngine;

public class AxeWeapon : ProjectileWeapon
{
    protected override float GetSpawnAngle()
    {
        int offset = currentAttackCount > 0 ? GetAmount() - currentAttackCount : 0;
        float angle = 90f - (5f * (offset + 1)) * Mathf.Sign(playerMovement.lastMoveDirection.x);
        // Debug.Log($"The {name} is spawning a projectile at a launch angle of {angle} degrees.");
        return angle;
    }

    protected override Vector2 GetSpawnOffset(float spawnAngle = 0f)
    {
        return new Vector2(
            Random.Range(currentStats.spawnVariance.xMin, currentStats.spawnVariance.xMax),
            Random.Range(currentStats.spawnVariance.yMin, currentStats.spawnVariance.yMax)
            );
    }

    // To easily implement additional specific scaling, you can use these override functions!
    // this could be replaced with something like the Resistances for Enemies too
    public override float GetAreaPercentage()
    {
        return base.GetAreaPercentage() * 1.3f;
    }

}
