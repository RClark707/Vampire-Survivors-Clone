using UnityEngine;

public class AuraWeapon : Weapon
{
    protected Aura currentAura;

    protected override void Update() { }

    public override bool LevelUp(bool updateUI = true)
    {
        if (!base.LevelUp(updateUI)) return false;

        OnEquip(); // this replaces setting the transform scale

        return true;
    }

    public override void OnEquip()
    {
        if (currentStats.auraPrefab)
        {
            if (currentAura) Destroy(currentAura.gameObject);
            currentAura = Instantiate(currentStats.auraPrefab, transform);
            currentAura.weapon = this;
            currentAura.owner = owner;
            float area = GetArea();
            currentAura.transform.localScale = new Vector3(area, area, area); // set area
        }
    }

    public override void OnUnequip()
    {
        if (currentAura) Destroy(currentAura);
    }
}
