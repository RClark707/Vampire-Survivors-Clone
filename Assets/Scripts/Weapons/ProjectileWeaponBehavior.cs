using UnityEngine;

public class ProjectileWeaponBehavior : MonoBehaviour
{
    protected Vector3 projectileDirection;
    public float projectileDuration;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        // Set the projectile's lifespan
        Destroy(gameObject, projectileDuration);
    }

    public void SetProjectileDirectionAndRotation(Vector3 dir)
    {
        projectileDirection = dir;

        float angle = Mathf.Atan2(projectileDirection.y, projectileDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90); // this hard codes a value, and assumes all weapons face UP?
    }
}
