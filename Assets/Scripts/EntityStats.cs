using UnityEngine;

public class EntityStats : ScriptableObject
{
    // Stats Common Among All Enemies & Player Characters
    public new string name;

    [SerializeField]
    float movementSpeed;
    public float MovementSpeed { get => movementSpeed; private set => movementSpeed = value; }

    [SerializeField]
    float maxHealth;
    public float MaxHealth { get => maxHealth; private set => maxHealth = value; }
}
