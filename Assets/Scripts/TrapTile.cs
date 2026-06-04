using UnityEngine;

public class TrapTile : MonoBehaviour
{
    public LevelManager levelManager;

    void OnTriggerEnter(Collider other)
    {
        DamagePlayer(other);
    }

    void OnTriggerStay(Collider other)
    {
        DamagePlayer(other);
    }

    private void DamagePlayer(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(1);
            }
            else
            {
                Debug.LogWarning("Player does not have PlayerHealth script.");
            }

            if (levelManager != null)
            {
                levelManager.PlayerHitTrap();
            }
        }
    }
}
