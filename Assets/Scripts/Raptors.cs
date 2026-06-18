using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class RaptorAI : MonoBehaviour
{
    private enum RaptorState
    {
        Waiting,
        Hunting,
        Attacking,
        Returning
    }

    [Header("References")]
    public NavMeshAgent agent;
    public Animator animator;
    
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip roarClip;
    public float roarVolume = 1f;

    [Header("Chase")]
    public float chaseSpeed = 5.5f;
    public float attackRange = 1.6f;
    public float updateTargetTime = 0.25f;

    [Header("Chase Limit")]
    public float maxDistanceFromNest = 10f;
    public float stopChasingPlayerDistance = 12f;
    public float returnStopDistance = 0.8f;

    [Header("Natural Movement")]
    public float sideMoveAmount = 1.0f;
    public float sideMoveSpeed = 3.0f;

    [Header("Attack")]
    public int damage = 1;
    public float attackCooldown = 1.5f;

    private Transform player;
    private Vector3 nestPosition;
    private RaptorState currentState = RaptorState.Waiting;

    private bool isAttacking = false;
    private float nextUpdateTime = 0f;
    private float nextAttackTime = 0f;

    private void Awake()
    {
        nestPosition = transform.position;

        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        if (agent != null)
        {
            agent.speed = chaseSpeed;
            agent.stoppingDistance = attackRange;
            agent.autoBraking = false;
        }
    }

    public void StartHunting(Transform target)
    {
        player = target;
        StartCoroutine(StartHuntingRoutine());
    }

    private IEnumerator StartHuntingRoutine()
    {
        if (agent != null)
        {
            agent.isStopped = true;
        }

        if (animator != null)
        {
            animator.SetTrigger("Roar");
        }

        PlayRoarSound();

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        yield return new WaitForSeconds(0.6f);

        currentState = RaptorState.Hunting;

        if (agent != null)
        {
            agent.isStopped = false;
        }
    }

    private void Update()
    {
        if (agent == null)
        {
            return;
        }

        if (currentState == RaptorState.Hunting)
        {
            HuntingUpdate();
        }
        else if (currentState == RaptorState.Returning)
        {
            ReturningUpdate();
        }

        UpdateAnimation();
    }

    private void HuntingUpdate()
    {
        if (player == null)
        {
            ReturnToNest();
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        float distanceFromNest = Vector3.Distance(transform.position, nestPosition);
        float playerDistanceFromNest = Vector3.Distance(player.position, nestPosition);

        if (distanceFromNest > maxDistanceFromNest || playerDistanceFromNest > stopChasingPlayerDistance)
        {
            ReturnToNest();
            return;
        }

        if (distanceToPlayer <= attackRange)
        {
            TryAttack();
        }
        else
        {
            ChasePlayer();
        }
    }

    private void ChasePlayer()
    {
        if (isAttacking)
        {
            return;
        }

        agent.isStopped = false;

        if (Time.time >= nextUpdateTime)
        {
            nextUpdateTime = Time.time + updateTargetTime;

            Vector3 targetPosition = GetNaturalTargetPosition();
            agent.SetDestination(targetPosition);
        }
    }

    private Vector3 GetNaturalTargetPosition()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        directionToPlayer.y = 0f;

        if (directionToPlayer.sqrMagnitude < 0.01f)
        {
            return player.position;
        }

        directionToPlayer.Normalize();

        Vector3 sideDirection = Vector3.Cross(Vector3.up, directionToPlayer).normalized;
        float sideOffset = Mathf.Sin(Time.time * sideMoveSpeed) * sideMoveAmount;

        Vector3 targetPosition = player.position + sideDirection * sideOffset;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(targetPosition, out hit, 2f, NavMesh.AllAreas))
        {
            return hit.position;
        }

        return player.position;
    }

    private void TryAttack()
    {
        if (Time.time < nextAttackTime || isAttacking)
        {
            return;
        }

        nextAttackTime = Time.time + attackCooldown;
        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;
        currentState = RaptorState.Attacking;

        agent.isStopped = true;

        Vector3 lookDirection = player.position - transform.position;
        lookDirection.y = 0f;

        if (lookDirection.sqrMagnitude > 0.01f)
        {
            transform.rotation = Quaternion.LookRotation(lookDirection);
        }

        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

        yield return new WaitForSeconds(0.4f);

        if (player != null)
        {
            float distance = Vector3.Distance(transform.position, player.position);

            if (distance <= attackRange + 0.4f)
            {
                PlayerHealth health = player.GetComponent<PlayerHealth>();

                if (health != null)
                {
                    health.TakeDamage(damage);
                }
            }
        }

        yield return new WaitForSeconds(0.5f);

        isAttacking = false;

        if (player != null)
        {
            float playerDistanceFromNest = Vector3.Distance(player.position, nestPosition);

            if (playerDistanceFromNest <= stopChasingPlayerDistance)
            {
                currentState = RaptorState.Hunting;
                agent.isStopped = false;
            }
            else
            {
                ReturnToNest();
            }
        }
        else
        {
            ReturnToNest();
        }
    }

    private void ReturnToNest()
    {
        currentState = RaptorState.Returning;
        isAttacking = false;

        if (agent != null)
        {
            agent.isStopped = false;
            agent.stoppingDistance = returnStopDistance;
            agent.SetDestination(nestPosition);
        }
    }

    private void ReturningUpdate()
    {
        float distanceToNest = Vector3.Distance(transform.position, nestPosition);

        if (distanceToNest <= returnStopDistance + 0.2f)
        {
            currentState = RaptorState.Waiting;

            agent.isStopped = true;
            agent.stoppingDistance = attackRange;

            if (animator != null)
            {
                animator.SetFloat("Speed", 0f);
            }
        }
    }

    private void UpdateAnimation()
    {
        if (animator == null || agent == null)
        {
            return;
        }

        float speed = agent.velocity.magnitude;
        animator.SetFloat("Speed", speed);
    }
    
    private void PlayRoarSound()
    {
        if (audioSource != null && roarClip != null)
        {
            audioSource.PlayOneShot(roarClip, roarVolume);
        }
    }
}
