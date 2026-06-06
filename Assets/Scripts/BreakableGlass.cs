using UnityEngine;

public class BreakableGlass : MonoBehaviour
{
    public float breakSpeed = 0.5f;

    private bool isBroken = false;

    void OnCollisionEnter(Collision collision)
    {
        CheckBreak(collision.gameObject);
    }

    void OnCollisionStay(Collision collision)
    {
        CheckBreak(collision.gameObject);
    }

    void CheckBreak(GameObject hitObject)
    {
        if (isBroken)
        {
            return;
        }

        Transform root = hitObject.transform.root;

        if (!hitObject.CompareTag("Player") && !root.CompareTag("Player"))
        {
            return;
        }

        RobotFreeAnim robotAnim = root.GetComponentInChildren<RobotFreeAnim>();

        if (robotAnim == null)
        {
            Debug.Log("Glass hit player, but RobotFreeAnim was not found.");
            return;
        }

        Animator anim = robotAnim.GetComponent<Animator>();

        if (anim == null)
        {
            Debug.Log("RobotFreeAnim found, but Animator was not found.");
            return;
        }

        bool isBallForm = anim.GetBool("Roll_Anim");

        if (!isBallForm)
        {
            Debug.Log("Hit glass, but player is not in ball form.");
            return;
        }

        Rigidbody playerRb = root.GetComponent<Rigidbody>();

        if (playerRb == null)
        {
            Debug.Log("Player Rigidbody was not found.");
            return;
        }

        float currentSpeed = playerRb.linearVelocity.magnitude;

        Debug.Log("Ball hit glass. Speed = " + currentSpeed);

        if (currentSpeed < breakSpeed)
        {
            Debug.Log("Ball speed is not enough.");
            return;
        }

        BreakGlass();
    }

    void BreakGlass()
    {
        isBroken = true;
        Debug.Log("Glass broken!");
        gameObject.SetActive(false);
    }
}
