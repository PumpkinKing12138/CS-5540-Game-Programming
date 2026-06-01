using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float speed = 10f;
    public float rotationSpeed = 1f;
    public float smoothSpeed = 0.1f;
    public float jumpHeight = 0.4f;
    public float gravity = 9.81f;
    public float airControl = 10f;
    public Transform cameraTransform;

    Vector3 input;
    Vector3 moveDir;
    CharacterController controller;

    float currentVelocity;
    int animState = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (!cameraTransform)
            cameraTransform = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
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
                moveDir.y = Mathf.Sqrt(2 * jumpHeight * gravity);
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
        
        //Debug.Log(moveDir.x + " " + moveDir.y + " " + moveDir.z);
        controller.Move(moveDir * speed * Time.deltaTime);
    }
}
