//Dan Ivanovas
using UnityEngine;

public abstract class Health : MonoBehaviour
{
    // Variables
    [Header("Health")]
    public int maxHealth = 10;

    private int currentHealth;

    public int CurrentHealth
    {
        get { return currentHealth; }
    }

    //when creating a script that will be inherited from you need virtual
    protected virtual void Awake()
    {

        //If you have been seeing Mathf.Max in my scripts and want to use it's very simple

        //The first value you pass in can't be changed but the second one can

        //So you can never go below 0 but and get as much health as you need

        //If you do want both values to be infinetely changable use .Clamp instead
        currentHealth = Mathf.Max(0, maxHealth);
    }


    //A simple TakeDamage Script that takes in a damage value and just subtracts it from current health
    public void TakeDamage(int damage)
    {
        if (damage <= 0 || currentHealth <= 0)
        {
            return;
        }

        currentHealth = Mathf.Max(0, currentHealth - damage);

        if (currentHealth <= 0)
        {

            //Very Sad
            Die();
        }
    }

    //Potentially could be useful if we wanted something simialr to btd Regen Balloons
    public void Heal(int amount)
    {
        if (amount <= 0 || currentHealth <= 0)
        {
            return;
        }

        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
    }

    protected abstract void Die();
}