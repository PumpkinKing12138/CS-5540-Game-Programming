using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 8f;
    public float ballMoveMultiplier = 2f;
    public float jumpForce = 6f;

    private Rigidbody rb;
    private bool isGrounded = true;

    private RobotFreeAnim robotAnim;
    private Animator robotAnimator;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        robotAnim = GetComponentInChildren<RobotFreeAnim>();

        if (robotAnim != null)
        {
            robotAnimator = robotAnim.GetComponent<Animator>();
        }
    }

    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveX, 0f, moveZ);

        float currentMoveSpeed = moveSpeed;

        if (IsBallForm())
        {
            currentMoveSpeed = moveSpeed * ballMoveMultiplier;
        }

        rb.AddForce(movement * currentMoveSpeed);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    bool IsBallForm()
    {
        if (robotAnimator == null)
        {
            return false;
        }

        return robotAnimator.GetBool("Roll_Anim");
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}
