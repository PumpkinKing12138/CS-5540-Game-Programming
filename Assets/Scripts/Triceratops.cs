using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Triceratops : MonoBehaviour
{
    [Header("Target")]
    public Transform player;

    [Header("Detection")]
    public float wakeUpRange = 6f;
    public float stopChaseRange = 20f;

    [Header("Movement")]
    public float chaseSpeed = 3.2f;
    public float rotationSpeed = 8f;

    [Header("Attack")]
    public int damage = 1;
    public float attackRange = 2.5f;
    public float attackCooldown = 1f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip roarClip;
    public float roarVolume = 1f;

    [Header("Animation Timing")]
    public float riseUpTime = 3.5f;
    public float roarTime = 6f;
    public float roarToWalkTime = 3f;
    public float fallTime = 5.5f;

    private Animator animator;
    private NavMeshAgent agent;
    private PlayerHealth playerHealth;

    private bool isBusy = false;
    private float lastAttackTime = -999f;

    private enum EnemyState
    {
        Sleeping,
        WakingUp,
        Chasing,
        Falling
    }

    private EnemyState currentState = EnemyState.Sleeping;

    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }

        if (player != null)
        {
            playerHealth = player.GetComponent<PlayerHealth>();

            if (playerHealth == null)
            {
                playerHealth = player.GetComponentInParent<PlayerHealth>();
            }

            if (playerHealth == null)
            {
                playerHealth = player.GetComponentInChildren<PlayerHealth>();
            }
        }

        if (agent != null)
        {
            agent.speed = chaseSpeed;
            agent.stoppingDistance = 1.5f;
            agent.isStopped = true;
            agent.updateRotation = false;
        }

        if (animator != null)
        {
            animator.ResetTrigger("WakeUp");
            animator.ResetTrigger("FallDown");
        }
    }

    void Update()
    {
        if (player == null || animator == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (currentState == EnemyState.Sleeping)
        {
            if (distanceToPlayer <= wakeUpRange && !isBusy)
            {
                StartCoroutine(WakeUpSequence());
            }
        }
        else if (currentState == EnemyState.Chasing)
        {
            if (distanceToPlayer >= stopChaseRange && !isBusy)
            {
                StartCoroutine(FallSequence());
            }
            else
            {
                ChasePlayer();
                TryDamagePlayer(distanceToPlayer);
            }
        }
    }

    IEnumerator WakeUpSequence()
    {
        isBusy = true;
        currentState = EnemyState.WakingUp;

        if (agent != null)
        {
            agent.isStopped = true;

            if (agent.isOnNavMesh)
            {
                agent.ResetPath();
            }
        }

        FacePlayer();

        animator.ResetTrigger("FallDown");
        animator.SetTrigger("WakeUp");

        yield return new WaitForSeconds(riseUpTime);

        PlayRoarSound();

        yield return new WaitForSeconds(roarTime + roarToWalkTime);

        currentState = EnemyState.Chasing;
        isBusy = false;

        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
        }
    }

    IEnumerator FallSequence()
    {
        isBusy = true;
        currentState = EnemyState.Falling;

        if (agent != null)
        {
            agent.isStopped = true;

            if (agent.isOnNavMesh)
            {
                agent.ResetPath();
            }
        }

        animator.ResetTrigger("WakeUp");
        animator.SetTrigger("FallDown");

        yield return new WaitForSeconds(fallTime);

        currentState = EnemyState.Sleeping;
        isBusy = false;
    }

    void ChasePlayer()
    {
        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
        }

        FacePlayer();
    }

    void TryDamagePlayer(float distanceToPlayer)
    {
        if (playerHealth == null) return;

        if (distanceToPlayer > attackRange) return;

        if (Time.time < lastAttackTime + attackCooldown) return;

        lastAttackTime = Time.time;

        playerHealth.TakeDamage(damage);

        Debug.Log("Triceratops damaged player.");
    }

    void PlayRoarSound()
    {
        if (audioSource == null || roarClip == null)
        {
            return;
        }

        audioSource.PlayOneShot(roarClip, roarVolume);
    }

    void FacePlayer()
    {
        if (player == null) return;

        Vector3 direction = player.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.01f) return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }
}
