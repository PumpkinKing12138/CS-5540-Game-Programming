using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;

    public Vector3 offset = new Vector3(0f, 8f, -10f);

    public float smoothSpeed = 5f;

    public float lookHeight = 1.5f;

    void LateUpdate()
    {
        if (player == null)
        {
            return;
        }

        Vector3 targetPosition = player.position + offset;

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            smoothSpeed * Time.deltaTime
        );

        Vector3 lookTarget = player.position + new Vector3(0f, lookHeight, 0f);

        transform.LookAt(lookTarget);
    }
}
