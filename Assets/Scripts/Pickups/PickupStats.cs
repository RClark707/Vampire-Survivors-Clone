using UnityEngine;

[CreateAssetMenu(fileName = "New Pickup Stats", menuName = "Stats/Pickup Stats")]
public class PickupStats : ScriptableObject
{
    public enum PickupType { Gold, Health, XP, Magnet }

    public PickupType pickupType;

    [SerializeField]
    float pickupStrength;
    public float PickupStrength { get => pickupStrength; private set => pickupStrength = value; }

    [SerializeField]
    float movementSpeed;
    public float MovementSpeed { get => movementSpeed; private set => movementSpeed = value; }
}
