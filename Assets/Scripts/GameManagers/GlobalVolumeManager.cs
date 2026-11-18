//Dan Ivanovas

using UnityEngine;
using UnityEngine.Rendering;

public class GlobalVolumeManager : MonoBehaviour
{
    public Volume damagePlayerVolume;


    public PlayerHealth playerHealth;


    public float damageIncrease = 0.15f;
    public float damageDuration = 10f;

    // Track damage and vignette strength
    private int previousHealth;
    private float damageTimer;
    private float damageWeight;

    void Start()
    {
        // in void Start try to store anything that will need to accessed on the first frame of the game

        // you can also add safety checks for everything and anything and I did this as an example
        if (playerHealth == null)
        {
            playerHealth = FindFirstObjectByType<PlayerHealth>();
        }

        if (playerHealth == null || damagePlayerVolume == null)
        {
            Debug.LogError("PlayerHealth or Damage Volume missing!");
            enabled = false;
            return;
        }

        previousHealth = playerHealth.CurrentHealth;
        damagePlayerVolume.weight = 0f;
    }

    void Update()
    {
        // Check if the player has taken damage
        if (playerHealth.CurrentHealth < previousHealth)
        {
            // Calculate how much health was lost
            int damageTaken = previousHealth - playerHealth.CurrentHealth;

            // Increase vignette strength based on damage
            damageWeight = Mathf.Clamp01(damagePlayerVolume.weight + damageIncrease * damageTaken);

            // Restart the fade timer
            damageTimer = damageDuration;
        }

        // Gradually fade the vignette
        if (damageTimer > 0f)
        {
            damageTimer = Mathf.Max(0f, damageTimer - Time.deltaTime);

            // Reduce volume weight over time
            damagePlayerVolume.weight = damageWeight *
                (damageTimer / Mathf.Max(0.01f, damageDuration));
        }
        else
        {
            // Remove the vignette when the timer ends
            damagePlayerVolume.weight = 0f;
        }

        // Store health for the next frame
        previousHealth = playerHealth.CurrentHealth;
    }
}