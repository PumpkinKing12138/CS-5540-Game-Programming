using UnityEngine;

public class MummyEnemy : MonoBehaviour
{
    [Header("Movement")]
    public float patrolSpeed = 1.5f;
    public float chaseSpeed = 3f;
    public float patrolDistance = 10f;
    public float detectionRange = 8f;

    [Header("References")]
    public Transform player;
    public Transform playerStartPoint;
    public LevelManager levelManager;

    private Vector3 startPosition;
    private Vector3 patrolTarget;
    private bool movingForward = true;

    void Start()
    {
        startPosition = transform.position;
        patrolTarget = startPosition + new Vector3(patrolDistance, 0f, 0f);
    }

    void Update()
    {
        if (player == null)
        {
            Patrol();
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRange)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }
    }

    void Patrol()
    {
        Vector3 currentTarget = movingForward ? patrolTarget : startPosition;
        MoveToward(currentTarget, patrolSpeed);

        if (Vector3.Distance(transform.position, currentTarget) < 0.2f)
        {
            movingForward = !movingForward;
        }
    }

    void ChasePlayer()
    {
        Vector3 targetPosition = new Vector3(
            player.position.x,
            transform.position.y,
            player.position.z
        );

        MoveToward(targetPosition, chaseSpeed);
    }

    void MoveToward(Vector3 target, float speed)
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            target,
            speed * Time.deltaTime
        );

        Vector3 direction = target - transform.position;
        direction.y = 0f;

        if (direction.magnitude > 0.1f)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ResetPlayer(other.gameObject);

            if (levelManager != null)
            {
                levelManager.PlayerCaughtByMummy();
            }
        }
    }

    private void ResetPlayer(GameObject playerObject)
    {
        if (playerStartPoint != null)
        {
            playerObject.transform.position = playerStartPoint.position;
        }

        Rigidbody rb = playerObject.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}
