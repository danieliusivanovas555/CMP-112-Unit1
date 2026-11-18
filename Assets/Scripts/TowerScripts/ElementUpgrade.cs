
public abstract class ElementUpgrade
{
    public abstract void ApplyTier(BaseTowerScript tower, int tier);
}

// Fire increases damage
public class FireElement : ElementUpgrade
{
    public override void ApplyTier(BaseTowerScript tower, int tier)
    {
        tower.damage += tier + 1;
    }
}

// Ice increases range
public class IceElement : ElementUpgrade
{
    public override void ApplyTier(BaseTowerScript tower, int tier)
    {
        tower.attackRange += (tier + 1) * 0.5f;
    }
}

// Lightning increases attack speed
public class LightningElement : ElementUpgrade
{
    public override void ApplyTier(BaseTowerScript tower, int tier)
    {
        tower.attackSpeed += (tier + 2) * 0.1f;
    }
}

// Earth increases damage and range
public class EarthElement : ElementUpgrade
{
    public override void ApplyTier(BaseTowerScript tower, int tier)
    {
        tower.damage += tier;
        tower.attackRange += 0.5f;
    }
}
