using System;

[Obsolete("No replacement, just don't use")]
public interface IItem
{
    // public bool MaxLevelReached();

    public bool IsUpgradeable();

    public void UpgradeItem();
}
