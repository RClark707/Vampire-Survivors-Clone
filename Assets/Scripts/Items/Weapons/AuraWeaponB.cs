using UnityEngine;

public class AuraWeaponB : WeaponB
{
    protected Aura currentAura;

    protected override void Update() { }

    public override bool LevelUp()
    {
        if (!base.LevelUp()) return false;

        if (currentAura)
        {
            currentAura.transform.localScale = new Vector3(currentStats.area, currentStats.area, currentStats.area); // this should be updated in the update function instead
        }
        return true;
    }

    public override void OnEquip()
    {
        if (currentStats.auraPrefab)
        {
            if (currentAura) Destroy(currentAura);
            currentAura = Instantiate(currentStats.auraPrefab, transform);
            currentAura.weapon = this;
            currentAura.owner = owner;
            currentAura.transform.localScale = new Vector3(currentStats.area, currentStats.area, currentStats.area); // set area
        }
    }

    public override void OnUnequip()
    {
        if (currentAura) Destroy(currentAura);
    }
}
