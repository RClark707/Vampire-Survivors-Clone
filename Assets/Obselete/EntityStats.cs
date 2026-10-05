using System;
using UnityEngine;

[Obsolete("We no longer use Scriptable Objects for Enemies or Props")]
public class EntityStats : ScriptableObject
{
    // Stats Common Among All Enemies & Player Characters
    public new string name;

    [SerializeField]
    Sprite icon;
    public Sprite Icon { get => icon; private set => icon = value; }

    [SerializeField]
    float movementSpeed;
    public float MovementSpeed { get => movementSpeed; private set => movementSpeed = value; }

    [SerializeField]
    float maxHealth;
    public float MaxHealth { get => maxHealth; private set => maxHealth = value; }
}
