//Dan Ivanovas
using UnityEngine;

public abstract class ElementUpgrade
{

    //OMGGGGGGGG I was wondering why the towers were seemingly getting stronger with the tier upgrades
    //Turns out I may have been stacking apply tier function in the old method
    //Also reduced ligthning aoe because that element is broken
    
    protected int baseDamage;
    protected float baseRange;
    protected float baseAttackSpeed;

    public void ApplyTier(BaseTowerScript tower, int tier)
    {
        //Save the original tower stats when first infused
        if (tier == 1)
        {
            baseDamage = tower.damage;
            baseRange = tower.attackRange;
            baseAttackSpeed = tower.attackSpeed;
        }

        //Apply the upgrade using the original stats
        ApplyUpgrade(tower, tier);
    }

    protected abstract void ApplyUpgrade(BaseTowerScript tower, int tier);
}

public class FireElement : ElementUpgrade
{
    protected override void ApplyUpgrade(BaseTowerScript tower, int tier)
    {
        //Increase damage by 50% per tier
        tower.damage = Mathf.RoundToInt(baseDamage * (1f + tier * 0.5f));
    }
}

public class IceElement : ElementUpgrade
{
    protected override void ApplyUpgrade(BaseTowerScript tower, int tier)
    {
        //Increase attack range by 25% per tier
        tower.attackRange = baseRange * (1f + tier * 0.25f);
    }
}

public class LightningElement : ElementUpgrade
{
    protected override void ApplyUpgrade(BaseTowerScript tower, int tier)
    {
        //Increase attack speed by 25% per tier
        tower.attackSpeed = baseAttackSpeed * (1f + tier * 0.25f);
    }
}

public class EarthElement : ElementUpgrade
{
    protected override void ApplyUpgrade(BaseTowerScript tower, int tier)
    {
        //Increase damage by 25% per tier
        tower.damage = Mathf.RoundToInt(baseDamage * (1f + tier * 0.25f));

        //Increase attack range by 15% per tier
        tower.attackRange = baseRange * (1f + tier * 0.15f);
    }
}