using UnityEngine;

public class PharaohHeartTrigger : MonoBehaviour
{
    public LevelManager levelManager;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("You touched Pharaoh's Heart.");

            if (levelManager != null)
            {
                levelManager.TryActivateHeart();
            }
        }
    }
}
