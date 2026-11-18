//Daniel Ferguson
using UnityEngine;

public class EnemyHealth : Health
{
    //store values related to enemies
    public int currencyReward = 10;

    //reference to the Currency Manager
    private CurrencyManager currencyManager;

    protected override void Awake()
    {
        base.Awake();

        //find the reference to currency manager

        currencyManager = FindFirstObjectByType<CurrencyManager>();
    }

    //if enemy dies add currency set above to player's total

    //destroy enemy on kill
    protected override void Die()
    {
        //reward player for kill before destroying
        currencyManager.AddCurrency(currencyReward);

        Destroy(gameObject);
    }
}
