using UnityEngine;

public class TrapTile : MonoBehaviour
{
    public Transform playerStartPoint;
    public LevelManager levelManager;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (playerStartPoint != null)
            {
                other.transform.position = playerStartPoint.position;
            }

            Rigidbody rb = other.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            if (levelManager != null)
            {
                levelManager.PlayerHitTrap();
            }
        }
    }
}
