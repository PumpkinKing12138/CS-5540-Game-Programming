using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyAI : MonoBehaviour
{
    public enum EnemyState {Navigate, Attack, Die}
    [Header("General Settings")]
    public EnemyState currentState = EnemyState.Navigate;
    public Transform targetBase;
    public int baseDamageValue = 10;
    public int reward = 1;
    

    [Header("Navigation Settings")]
    public Transform turrent;
    public float rotationSpeed = 30f;
    public float detectionRange = 10f;

    [Header("Attack Settings")]
    public bool canAttack = true;
    public GameObject projectilePrefab;
    public Transform firePoint;
    public int fireRate = 2;
    
    [Header("Die Settings")]
    public int health = 100;
    public GameObject destroyPrefab;
    public Slider healthSlider;

    NavMeshAgent agent;
    bool isDead;
    float fireCooldown = 0;
    Transform target;
    Quaternion intialTurrentRotation;
    int maxHealth;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (!targetBase)
        {
            targetBase = GameObject.FindGameObjectWithTag("Target").transform;

            if (!targetBase)
            {
                Debug.Log("No targetbase");
                return;
            }
        }

        agent.SetDestination(targetBase.position);
        
        if (turrent)
            intialTurrentRotation = turrent.localRotation;

        maxHealth = health;
        if (healthSlider)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = health;
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case EnemyState.Navigate:
                Navigate();
                break;
            case EnemyState.Attack:
                if (canAttack)
                    Attack();
                else
                    currentState = EnemyState.Navigate;
                break;
            case EnemyState.Die:
                Die();
                break;
        }
    }

    void Navigate()
    {
        // agent.SetDestination(targetBase.position);

        if (canAttack)
            FindNearestTower();

        if (turrent)
            turrent.localRotation = Quaternion.Slerp(turrent.localRotation, intialTurrentRotation, rotationSpeed * Time.deltaTime);
    }

    void Attack()
    {
        if (target == null || Vector3.Distance(transform.position, target.position) > detectionRange)
        {
            target = null;
            currentState = EnemyState.Navigate;
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

        agent.isStopped = true;

        if (destroyPrefab)
            Instantiate(destroyPrefab, transform.position, transform.rotation);

        Destroy(gameObject);
        MoneyManager.Instance.GetMoney(reward);
        isDead = true;
    }

    void FindNearestTower()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRange);
        Transform nearestTower = null;
        float minDis = Mathf.Infinity;
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Tower"))
            {
                float dis = Vector3.Distance(transform.position, collider.transform.position);
                if (dis < minDis)
                {
                    minDis = dis;
                    nearestTower = collider.transform;
                }
            }
        }

        if (nearestTower)
        {
            target = nearestTower;
            currentState = EnemyState.Attack;
            return;
        }
    }

    void Shoot()
    {
        if (!canAttack)
            return;

        var bullet = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        BulletBehavior bulletBehavior = bullet.GetComponent<BulletBehavior>();
        if (bulletBehavior)
        {
            var targetTurrent = target.transform.Find("Turrent");
            if (targetTurrent)
                bulletBehavior.SetTarget(targetTurrent);
            else
                bulletBehavior.SetTarget(target);
        }   
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (healthSlider)
        {
            healthSlider.value = health;
        }
        if (health <= 0)
        {
            currentState = EnemyState.Die;
            health = 0;
        }
    }

    bool HasLineofSight(Transform target)
    {
        RaycastHit hit;
        Vector3 direction = (target.position - firePoint.position).normalized;
        if (Physics.Raycast(firePoint.position, direction, out hit, detectionRange))
        {
            if (hit.collider.CompareTag("Tower"))
            {
                Debug.Log("Tower in sight");
                return true;
            }
        }
        return false;
    }

    public int GetEnemyDamageValue()
    {
        return baseDamageValue;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Bullet"))
        {
            BulletBehavior bulletBehavior = collision.gameObject.GetComponent<BulletBehavior>();
            if (bulletBehavior)
            {
                int bulletDamage = bulletBehavior.GetDamageValue();
                TakeDamage(bulletDamage);
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
