using UnityEngine;

public class SealCollectible : MonoBehaviour
{
    public LevelManager levelManager;

    private bool collected = false;

    void OnTriggerEnter(Collider other)
    {
        if (collected)
        {
            return;
        }

        if (other.CompareTag("Player"))
        {
            collected = true;

            Debug.Log(gameObject.name + " collected.");

            if (levelManager != null)
            {
                levelManager.CollectSeal();
            }

            gameObject.SetActive(false);
        }
    }

    public void ResetSeal()
    {
        collected = false;
        gameObject.SetActive(true);
    }
}
