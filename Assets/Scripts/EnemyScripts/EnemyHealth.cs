//Dan Ivanovas
using UnityEngine;

public class EnemyHealth : Health
{
    //Store any values related to the enemy here
    public int currencyReward = 10;

    //Reference to the Currency Manager
    private CurrencyManager currencyManager;

    protected override void Awake()
    {
        base.Awake();

        //enemy is a prefab so when it spawn it needs to find a reference to currency manager

        // you are probably going to use something like this for towers potentially

        currencyManager = FindFirstObjectByType<CurrencyManager>();
    }

    //If enemy dies add currency it has been set

    //Then kill it :(
    protected override void Die()
    {
        // Reward player for defeating enemy
        currencyManager.AddCurrency(currencyReward);

        Destroy(gameObject);
    }
}
