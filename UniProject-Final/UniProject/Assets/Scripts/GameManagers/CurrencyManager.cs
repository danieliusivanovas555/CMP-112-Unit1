//Dan Ivanovas
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    //Variables
    public int startingCurrency = 0;
    public int roundCompletionReward = 100;

    public int CurrentCurrency { get; private set; }

    void Awake()
    {
       
        CurrentCurrency = startingCurrency;
    }

    //Check WaveSystem script to see how to add currency (works similar to the health script)
    public void AddCurrency(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        CurrentCurrency += amount;
    }
    //You'll need this for tower upgrades spending
    public bool SpendCurrency(int amount)
    {
        if (amount < 0 || CurrentCurrency < amount)
        {
            return false;
        }

        CurrentCurrency -= amount;
        return true;
    }
}