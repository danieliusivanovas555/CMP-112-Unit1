//Daniel Ferguson
using UnityEngine;

//set class to change between different elements later
[RequireComponent(typeof(EnemyHealth))]
public class EnemyElement : MonoBehaviour
{
    [Header("Element Settings")]
    public TowerElement element = TowerElement.None;
    public TowerElement counterElement = TowerElement.None;

    //set weak/strong multipliers with limits to stop weak hits dealing more than normal or strong hits dealing less
    [Header("Damage Multipliers")]
    [Range(0f, 1f)]
    public float resistanceMultiplier = 0.5f;

    [Min(1f)]
    public float counterMultiplier = 1.5f;

    //set variables for hp and left over damage from hits
    private EnemyHealth health;
    private float remainingDamage;

    //get enemy health on creation
    void Awake()
    {
        health = GetComponent<EnemyHealth>();
    }


    public void TakeElementDamage(int damage, TowerElement attackingElement)
    {
        float currentMultiplier = 1f;

        //take less damage from the same element
        if (attackingElement == element && element != TowerElement.None)
        {
            currentMultiplier = resistanceMultiplier;
        }

        //take extra damage from set element counter
        else if (attackingElement == counterElement && counterElement != TowerElement.None)
        {
            currentMultiplier = counterMultiplier;
        }

        float totalDamage = damage * currentMultiplier + remainingDamage;

        int finalDamage = Mathf.FloorToInt(totalDamage);

        //save fractional damage for the next hit to stop decimals used for hp (could be helpful for current hp based towers/effects added later)
        remainingDamage = totalDamage - finalDamage;

        if (finalDamage > 0)
        {
            health.TakeDamage(finalDamage);
        }
    }
}