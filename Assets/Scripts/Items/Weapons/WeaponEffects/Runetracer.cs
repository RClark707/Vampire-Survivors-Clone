using UnityEngine;

public class Runetracer : Projectile
{
    protected override void FixedUpdate()
    {
        // base.FixedUpdate() // only matters for kinematic objects

        RealignRotation();
    }

    protected void RealignRotation()
    {
        float aimAngle;

        // Debug.Log($"Nearest Enemy: {weapon.nearestEnemy.transform.position}");
        Vector2 direction = rb.linearVelocity;
        aimAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // This line rotates our projectile so that linearVelocity sends it in the aimAngle direction
        // This assumes our projectile sprite faces UP

        if (spriteDirection == SpriteDirection.Up || spriteDirection == SpriteDirection.Down)
        {
            aimAngle -= 90f;
        }

        transform.rotation = Quaternion.Euler(0f, 0f, aimAngle);
    }
}
