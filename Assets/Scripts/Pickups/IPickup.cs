using UnityEngine;

public interface IPickup
{
    // we can use interfaces to guarantee that you have access to specific functions
    // a common use case is checking collisions

    void Collect(Player player);

    void Follow(Transform target);
}
