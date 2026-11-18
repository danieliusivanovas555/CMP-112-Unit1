//Dan Ivanovas
using UnityEngine;

public class EnemyEffects : MonoBehaviour
{
    private EnemyHealth health;
    private EnemyWaypointMovement movement;
    private EnemyElement enemyElement;

    private float normalSpeed;

    // Fire
    private int burnDamage;
    private float burnEndTime;
    private float nextBurnTick;

    // Ice
    private float slowAmount = 1f;
    private float slowEndTime;

    // Earth
    private float stunEndTime;

    void Awake()
    {
        //Get the enemy components
        health = GetComponent<EnemyHealth>();
        movement = GetComponent<EnemyWaypointMovement>();
        enemyElement = GetComponent<EnemyElement>();

        normalSpeed = movement.moveSpeed;
    }

    void Update()
    {
        //Fire damage over time
        if (nextBurnTick > 0f && Time.time >= nextBurnTick && nextBurnTick <= burnEndTime)
        {
            if (enemyElement != null)
            {
                enemyElement.TakeElementDamage(burnDamage, TowerElement.Fire);
            }
            else
            {
                health.TakeDamage(burnDamage);
            }

            nextBurnTick += 1f;
        }

        //Return to normal speed unless slowed
        float currentSpeed = normalSpeed;

        if (Time.time < slowEndTime)
        {
            currentSpeed *= slowAmount;
        }

        //Stun takes priority over slowing
        if (Time.time < stunEndTime)
        {
            currentSpeed = 0f;
        }

        movement.moveSpeed = currentSpeed;
    }

    public void ApplyBurn(int damage, float duration)
    {
        //Apply burning damage and refresh its duration
        burnDamage = damage;

        if (Time.time >= burnEndTime)
        {
            nextBurnTick = Time.time + 1f;
        }

        burnEndTime = Time.time + duration;
    }

    public void ApplySlow(float amount, float duration)
    {
        //Reduce the enemy movement speed
        slowAmount = Mathf.Clamp01(amount);
        slowEndTime = Time.time + duration;

        if (Time.time >= stunEndTime)
        {
            movement.moveSpeed = normalSpeed * slowAmount;
        }
    }

    public void ApplyStun(float duration)
    {
        //Stop the enemy from moving
        stunEndTime = Time.time + duration;

        movement.moveSpeed = 0f;
    }
}
