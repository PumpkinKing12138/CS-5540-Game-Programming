using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonController : MonoBehaviour
{
    public float speed = 8f;
    public float ballSpeedMultiplier = 3f;

    public float robotJumpHeight = 0.8f;
    public float ballJumpHeight = 0.3f;

    public float transformLockTime = 0.5f;

    public float rotationSpeed = 1f;
    public float smoothSpeed = 0.1f;
    public float gravity = 9.81f;
    public float airControl = 10f;
    public Transform cameraTransform;

    public TrailRenderer ballTrail;

    Vector3 input;
    Vector3 moveDir;
    CharacterController controller;

    float currentVelocity;
    float transformLockTimer = 0f;
    int animState = 0;

    private RobotFreeAnim robotAnim;
    private Animator robotAnimator;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        if (!cameraTransform)
        {
            cameraTransform = Camera.main.transform;
        }

        robotAnim = GetComponentInChildren<RobotFreeAnim>();

        if (robotAnim != null)
        {
            robotAnimator = robotAnim.GetComponent<Animator>();
        }

        if (ballTrail == null)
        {
            ballTrail = GetComponentInChildren<TrailRenderer>();
        }

        if (ballTrail != null)
        {
            ballTrail.emitting = false;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            transformLockTimer = transformLockTime;
        }

        if (transformLockTimer > 0f)
        {
            transformLockTimer -= Time.deltaTime;

            input = Vector3.zero;
            moveDir.x = 0f;
            moveDir.z = 0f;

            if (controller.isGrounded)
            {
                moveDir.y = 0f;
            }
            else
            {
                moveDir.y -= gravity * Time.deltaTime;
            }

            controller.Move(moveDir * Time.deltaTime);

            if (ballTrail != null)
            {
                ballTrail.emitting = false;
            }

            return;
        }

        float moveH = Input.GetAxis("Horizontal");
        float moveV = Input.GetAxis("Vertical");

        input = new Vector3(moveH, 0f, moveV);
        input.Normalize();

        if (controller.isGrounded)
        {
            moveDir = input;
            animState = 0;

            if (input.magnitude >= 1.0f)
            {
                animState = 1;

                float rotationAngle = Mathf.Atan2(input.x, input.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
                float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, rotationAngle, ref currentVelocity, smoothSpeed);
                transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);

                Vector3 moveD = Quaternion.Euler(0f, rotationAngle, 0f) * Vector3.forward;
                moveDir = moveD.normalized * rotationSpeed;

                if (Input.GetButton("Fire1"))
                {
                    animState = 3;
                }
            }
            else if (Input.GetButtonDown("Fire1"))
            {
                animState = 4;
            }

            if (Input.GetButton("Jump"))
            {
                float currentJumpHeight = robotJumpHeight;

                if (IsBallForm())
                {
                    currentJumpHeight = ballJumpHeight;
                }

                moveDir.y = Mathf.Sqrt(2 * currentJumpHeight * gravity);
                animState = 2;
            }
            else
            {
                moveDir.y = 0.0f;
            }
        }
        else
        {
            moveDir.y -= gravity * Time.deltaTime;
        }

        float currentSpeed = speed;

        if (IsBallForm())
        {
            currentSpeed = speed * ballSpeedMultiplier;
        }

        Vector3 horizontalMove = new Vector3(moveDir.x, 0f, moveDir.z) * currentSpeed;
        Vector3 verticalMove = new Vector3(0f, moveDir.y, 0f);

        controller.Move((horizontalMove + verticalMove) * Time.deltaTime);

        UpdateBallTrail();
    }

    bool IsBallForm()
    {
        if (robotAnimator == null)
        {
            return false;
        }

        return robotAnimator.GetBool("Roll_Anim");
    }

    void UpdateBallTrail()
    {
        if (ballTrail == null)
        {
            return;
        }

        bool isMoving = input.magnitude > 0.1f;
        bool shouldShowTrail = IsBallForm() && isMoving;

        ballTrail.emitting = shouldShowTrail;
    }
}
