
using UnityEngine;

[RequireComponent(typeof(EnemyHealth))]
public class EnemyElement : MonoBehaviour
{
    [Header("Element Settings")]
    public TowerElement element = TowerElement.None;
    public TowerElement counterElement = TowerElement.None;

    [Header("Damage Multipliers")]
    [Range(0f, 1f)]
    public float resistanceMultiplier = 0.5f;

    [Min(1f)]
    public float counterMultiplier = 1.5f;

    private EnemyHealth health;
    private float remainingDamage;

    void Awake()
    {
        health = GetComponent<EnemyHealth>();
    }

    public void TakeElementDamage(int damage, TowerElement attackingElement)
    {
        float multiplier = 1f;

        // Take less damage from the same element
        if (element != TowerElement.None && attackingElement == element)
        {
            multiplier = resistanceMultiplier;
        }

        // Take extra damage from the assigned counter element
        else if (counterElement != TowerElement.None &&
                 attackingElement == counterElement)
        {
            multiplier = counterMultiplier;
        }

        float totalDamage = damage * multiplier + remainingDamage;

        int finalDamage = Mathf.FloorToInt(totalDamage);

        // Save fractional damage for the next hit
        remainingDamage = totalDamage - finalDamage;

        if (finalDamage > 0)
        {
            health.TakeDamage(finalDamage);
        }
    }
}