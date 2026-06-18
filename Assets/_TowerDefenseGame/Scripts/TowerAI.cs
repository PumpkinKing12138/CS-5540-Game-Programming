using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Rendering;

public class TowerAI : MonoBehaviour
{
    public enum TowerState {Patrol, Attack, Die}
    public TowerState currentState = TowerState.Patrol;

    [Header("Patrol Settings")]
    public Transform turrent;
    public float rotationSpeed = 30f;
    public float maxRotationAngle = 90f;
    public float detectionRange = 10f;

    [Header("Attack Settings")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public int fireRate = 2;
    
    [Header("Die Settings")]
    public int health = 100;
    public GameObject destroyPrefab;

    [Header("General Settings")]
    public GameObject buildPrefab;

    bool isDead;
    float fireCooldown = 0;
    Transform target;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (buildPrefab)
            Instantiate(buildPrefab, transform.position, transform.rotation);
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case TowerState.Patrol:
                Patrol();
                break;
            case TowerState.Attack:
                Attack();
                break;
            case TowerState.Die:
                Die();
                break;
        }
    }

    void Patrol()
    {
        float angle = Mathf.PingPong(rotationSpeed * Time.time, maxRotationAngle * 2) - maxRotationAngle;
        turrent.localRotation = Quaternion.Euler(0, angle, 0);

        LookForEnemies();
    }

    void Attack()
    {
        if (target == null || Vector3.Distance(transform.position, target.position) > detectionRange)
        {
            target = null;
            currentState = TowerState.Patrol;
            return;
        }

        Vector3 direction = target.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        turrent.rotation = Quaternion.Slerp(turrent.rotation, lookRotation, rotationSpeed * Time.deltaTime);

        if (fireCooldown <= 0)
        {
            if (HasLineofSight(target))
                Shoot();
            fireCooldown = 1.0f / fireRate;
        }

        fireCooldown -= Time.deltaTime;
    }

    void Die()
    {
        if (isDead)
            return;

        if (destroyPrefab)
            Instantiate(destroyPrefab, transform.position, transform.rotation);

        Destroy(gameObject, 1);
        isDead = true;
    }

    void LookForEnemies()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRange);
        Transform nearestEnemy = null;
        float minDis = Mathf.Infinity;
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                float dis = Vector3.Distance(transform.position, collider.transform.position);
                if (dis < minDis)
                {
                    minDis = dis;
                    nearestEnemy = collider.transform;
                }
            }
        }

        if (nearestEnemy)
        {
            target = nearestEnemy;
            currentState = TowerState.Attack;
        }
    }

    void Shoot()
    {
        var bullet = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        BulletBehavior bulletBehavior = bullet.GetComponent<BulletBehavior>();
        if (bulletBehavior)
            bulletBehavior.SetTarget(target);
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            currentState = TowerState.Die;
        }
    }

    bool HasLineofSight(Transform target)
    {
        RaycastHit hit;
        Vector3 direction = (target.position - firePoint.position).normalized;
        if (Physics.Raycast(firePoint.position, direction, out hit, detectionRange))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                Debug.Log("Enemy in sight");
                return true;
            }
        }
        return false;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("EnemyBullet"))
        {
            BulletBehavior bulletBehavior = collision.gameObject.GetComponent<BulletBehavior>();
            if (bulletBehavior)
            {
                int bulletDamage = bulletBehavior.GetDamageValue();
                TakeDamage(bulletDamage);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }

    void OnDrawGizmos()
    {
        Vector3 lineEnd = firePoint.position + (firePoint.forward * detectionRange);
        Debug.DrawLine(firePoint.position, lineEnd, Color.green);
    }
}
