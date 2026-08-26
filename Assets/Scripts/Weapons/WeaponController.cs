using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("Weapon Stats")]
    public GameObject weaponPrefab;
    public float baseDamage; // { get; private set; }
    public float baseSpeed; // { get; private set; }
    public float baseCooldown; // { get; private set; }
    float currentCooldown;
    public int basePierceCount; // { get; private set; }

    protected PlayerMovement pm;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentCooldown = baseCooldown;
        pm = FindAnyObjectByType<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        currentCooldown -= Time.deltaTime;

        if (currentCooldown > 0f)
        {
            return;
        }

        currentCooldown = baseCooldown;

        // Then Use the Weapon
        Use();
    }

    // This method is meant to be overridden
    // and we can return a bool if we want to check that it runs successfully
    protected virtual void Use()
    {
        Debug.Log($"Using {gameObject.name}!");
        return;
    }
}
