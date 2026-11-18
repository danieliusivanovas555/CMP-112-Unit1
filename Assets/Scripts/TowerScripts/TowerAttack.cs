//Dan Ivanovas
using UnityEngine;

public class TowerAttack : MonoBehaviour
{

    //Variables

    public TowerProjectile projectilePrefab;
    public TowerProjectile fireProjectile;
    public TowerProjectile iceProjectile;
    public TowerProjectile lightningProjectile;
    public TowerProjectile earthProjectile;


    public Transform firePoint;


    public Transform rotatingPivot;
    public float rotationSpeed = 180f;
    public float rotationOffset = 0f;

    private BaseTowerScript tower;
    private EnemyEffects currentTarget;

    private float attackTimer;
    private float searchTimer;

    void Start()
    {
        //Get the tower script
        tower = GetComponent<BaseTowerScript>();
    }

    void Update()
    {
        //Stop if the tower hasn't been placed
        if (!tower.IsPlaced)
        {
            return;
        }

        attackTimer -= Time.deltaTime;

        //Check if the current enemy is still in range
        if (currentTarget != null)
        {
            Vector3 distance = currentTarget.transform.position - transform.position;

            if (distance.sqrMagnitude > tower.attackRange * tower.attackRange)
            {
                currentTarget = null;
            }
        }

        //Find another enemy if the current target is lost
        if (currentTarget == null)
        {
            searchTimer -= Time.deltaTime;

            if (searchTimer <= 0f)
            {
                currentTarget = FindTarget();
                searchTimer = 0.2f;
            }
        }

        //Stop if no enemies are available
        if (currentTarget == null)
        {
            return;
        }

        //Rotate the tower towards the enemy
        RotateToTarget();

        //Fire based on the tower attack speed
        if (attackTimer <= 0f)
        {
            Fire();
            attackTimer = 1f / Mathf.Max(0.01f, tower.attackSpeed);
        }
    }

    EnemyEffects FindTarget()
    {
        //Find all colliders inside the tower attack range
        Collider[] colliders = Physics.OverlapSphere(
            transform.position,
            tower.attackRange
        );

        EnemyEffects firstEnemy = null;
        float highestProgress = -1f;

        foreach (Collider collider in colliders)
        {
            //Check if the collider belongs to an enemy
            EnemyEffects enemy = collider.GetComponentInParent<EnemyEffects>();

            if (enemy == null)
            {
                continue;
            }

            EnemyWaypointMovement movement = enemy.GetComponent<EnemyWaypointMovement>();

            //Check if the enemy is inside the attack range
            Vector3 distance = enemy.transform.position - transform.position;

            if (distance.sqrMagnitude > tower.attackRange * tower.attackRange)
            {
                continue;
            }

            //Find the enemy furthest along the waypoint path
            if (movement.PathProgress > highestProgress)
            {
                highestProgress = movement.PathProgress;
                firstEnemy = enemy;
            }
        }

        return firstEnemy;
    }

    void RotateToTarget()
    {
        //Calculate the direction towards the enemy
        Vector3 direction = currentTarget.transform.position - rotatingPivot.position;

        //Only rotate horizontally
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f)
        {
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        targetRotation *= Quaternion.Euler(0f, rotationOffset, 0f);

        //Smoothly rotate the tower towards the enemy
        rotatingPivot.rotation = Quaternion.RotateTowards(rotatingPivot.rotation, targetRotation,rotationSpeed * Time.deltaTime);
    }

    void Fire()
    {
        //Choose the projectile based on the tower element
        TowerProjectile selectedProjectile = projectilePrefab;

        switch (tower.currentElement)
        {
            case TowerElement.Fire:
                selectedProjectile = fireProjectile;
                break;

            case TowerElement.Ice:
                selectedProjectile = iceProjectile;
                break;

            case TowerElement.Lightning:
                selectedProjectile = lightningProjectile;
                break;

            case TowerElement.Earth:
                selectedProjectile = earthProjectile;
                break;
        }

        //Use the normal projectile if an elemental one isn't assigned
        if (selectedProjectile == null)
        {
            selectedProjectile = projectilePrefab;
        }

        //Spawn the projectile at the firing point
        TowerProjectile projectile = Instantiate(selectedProjectile, firePoint.position,firePoint.rotation);

        projectile.Launch(currentTarget, tower);
    }

    void OnDrawGizmosSelected()
    {
        //Display the tower attack range in the Scene view
        BaseTowerScript stats = GetComponent<BaseTowerScript>();

        if (stats != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, stats.attackRange);
        }
    }
}