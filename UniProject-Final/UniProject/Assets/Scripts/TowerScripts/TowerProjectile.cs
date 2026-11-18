
//Dan Ivanovas
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SphereCollider))]
public class TowerProjectile : MonoBehaviour
{
    public float speed = 15f;
    public float turnSpeed = 8f;
    public float lifetime = 5f;
    public float targetHeight = 0.5f;

    public AudioClip normalSound;
    public AudioClip fireSound;
    public AudioClip iceSound;
    public AudioClip lightningSound;
    public AudioClip earthSound;
    public AudioClip shatterSound;

    [Range(0f, 1f)]
    public float soundVolume = 0.75f;

    public ParticleSystem shatterParticles;

    public int burnDamage = 1;
    public float burnDuration = 3f;

    public float lightningRadius = 2.5f;

    public float slowAmount = 0.5f;
    public float slowDuration = 2f;

    public float stunDuration = 0.75f;

    private Rigidbody rb;
    private EnemyEffects target;
    private TowerElement element;
    private int damage;
    private bool hasHit;

    void Awake()
    {
        //Get the projectile Rigidbody
        rb = GetComponent<Rigidbody>();
    }

    public void Launch(EnemyEffects enemy, BaseTowerScript tower)
    {
        //Get the enemy and tower information
        target = enemy;
        damage = tower.damage;
        element = tower.currentElement;

        //Calculate the direction towards the enemy
        Vector3 direction = (target.transform.position + Vector3.up * targetHeight - transform.position).normalized;

        //Launch the projectile
        rb.AddForce(direction * speed, ForceMode.VelocityChange);

        PlayFiringSound();

        //Destroy the projectile after its lifetime
        Destroy(gameObject, lifetime);
    }

    void FixedUpdate()
    {
        //Destroy the projectile if the enemy no longer exists
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        //Calculate the direction towards the moving enemy
        Vector3 direction = (target.transform.position + Vector3.up * targetHeight - transform.position).normalized;

        Vector3 desiredVelocity = direction * speed;
        Vector3 velocityChange = desiredVelocity - rb.linearVelocity;

        //Adjust the projectile movement towards the enemy
        rb.AddForce(velocityChange * Mathf.Clamp01(turnSpeed * Time.fixedDeltaTime), ForceMode.VelocityChange);
    }

    void PlayFiringSound()
    {
        //Select the sound based on the projectile element
        AudioClip selectedSound = normalSound;

        switch (element)
        {
            case TowerElement.Fire:
                selectedSound = fireSound;
                break;

            case TowerElement.Ice:
                selectedSound = iceSound;
                break;

            case TowerElement.Lightning:
                selectedSound = lightningSound;
                break;

            case TowerElement.Earth:
                selectedSound = earthSound;
                break;
        }

        if (selectedSound != null)
        {
            AudioSource.PlayClipAtPoint(selectedSound, transform.position, soundVolume);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        //Prevent the projectile from hitting multiple enemies
        if (hasHit)
        {
            return;
        }

        //Only detect enemy colliders
        EnemyEffects enemy = other.GetComponentInParent<EnemyEffects>();

        if (enemy == null)
        {
            return;
        }

        hasHit = true;

        //Damage the enemy
        HitEnemy(enemy);

        //Get the position where the projectile hit
        Vector3 hitPosition = other.ClosestPoint(transform.position);

        //Play the shattering sound
        if (shatterSound != null)
        {
            AudioSource.PlayClipAtPoint(shatterSound, hitPosition, soundVolume);
        }

        //Spawn the shattering particles
        if (shatterParticles != null)
        {
            ParticleSystem particles = Instantiate(shatterParticles, hitPosition, Quaternion.identity);

            particles.Play();

            Destroy(particles.gameObject, 5f);
        }

        //Destroy the projectile
        Destroy(gameObject);
    }

    void HitEnemy(EnemyEffects enemy)
    {
        //Apply the effect based on the projectile element
        switch (element)
        {
            case TowerElement.Fire:
                enemy.ApplyBurn(burnDamage, burnDuration);
                break;

            case TowerElement.Ice:
                enemy.ApplySlow(slowAmount, slowDuration);
                break;

            case TowerElement.Lightning:
                LightningAttack(enemy);
                break;

            case TowerElement.Earth:
                enemy.ApplyStun(stunDuration);
                break;
        }

        //Damage the enemy based on its elemental resistance
        DamageEnemy(enemy);
    }

    void DamageEnemy(EnemyEffects enemy)
    {
        EnemyElement enemyElement = enemy.GetComponent<EnemyElement>();

        if (enemyElement != null)
        {
            enemyElement.TakeElementDamage(damage, element);
        }
        else
        {
            enemy.GetComponent<EnemyHealth>().TakeDamage(damage);
        }
    }

    void LightningAttack(EnemyEffects targetEnemy)
    {
        //Find enemies near the original target
        Collider[] colliders = Physics.OverlapSphere(targetEnemy.transform.position, lightningRadius);

        List<EnemyEffects> hitEnemies = new List<EnemyEffects>();

        //Prevent damaging the original enemy twice
        hitEnemies.Add(targetEnemy);

        foreach (Collider collider in colliders)
        {
            EnemyEffects enemy = collider.GetComponentInParent<EnemyEffects>();

            if (enemy == null)
            {
                continue;
            }

            //Prevent damaging the same enemy multiple times
            if (!hitEnemies.Contains(enemy))
            {
                hitEnemies.Add(enemy);
                DamageEnemy(enemy);
            }
        }
    }
}
