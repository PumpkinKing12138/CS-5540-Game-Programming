using UnityEngine;
using FlexibleGlassDestructor;

[RequireComponent(typeof(CharacterController))]
public class PlayerGlassBreaker : MonoBehaviour
{
    public float minBallSpeed = 2f;
    public float glassDamage = 30f;
    public float hitCooldown = 0.25f;

    private CharacterController controller;
    private Animator robotAnimator;
    private float nextHitTime = 0f;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        RobotFreeAnim robotAnim = GetComponentInChildren<RobotFreeAnim>();

        if (robotAnim != null)
        {
            robotAnimator = robotAnim.GetComponent<Animator>();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        TryBreakGlass(other, other.ClosestPoint(transform.position));
    }

    void OnTriggerStay(Collider other)
    {
        TryBreakGlass(other, other.ClosestPoint(transform.position));
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        TryBreakGlass(hit.collider, hit.point);
    }

    void TryBreakGlass(Collider other, Vector3 hitPoint)
    {
        if (Time.time < nextHitTime)
        {
            return;
        }

        FlexibleGlass glass = other.GetComponentInParent<FlexibleGlass>();

        if (glass == null)
        {
            return;
        }

        if (!IsBallForm())
        {
            Debug.Log("Hit glass, but player is not in ball form.");
            return;
        }

        float currentSpeed = new Vector3(controller.velocity.x, 0f, controller.velocity.z).magnitude;

        if (currentSpeed < minBallSpeed)
        {
            Debug.Log("Ball hit glass, but speed is too low: " + currentSpeed);
            return;
        }

        Vector3 direction = controller.velocity.normalized;

        if (direction.sqrMagnitude < 0.01f)
        {
            direction = transform.forward;
        }

        glass.TakeDamage(hitPoint, direction, glassDamage);

        nextHitTime = Time.time + hitCooldown;

        Debug.Log("Ball hit FlexibleGlass. Speed = " + currentSpeed);
    }

    bool IsBallForm()
    {
        if (robotAnimator == null)
        {
            return false;
        }

        return robotAnimator.GetBool("Roll_Anim");
    }
}
