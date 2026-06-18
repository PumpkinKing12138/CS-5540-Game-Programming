using UnityEngine;

public class RaptorEggTrigger : MonoBehaviour
{
    [Header("Raptors to activate")]
    public GameObject[] raptors;

    [Header("Trigger Settings")]
    public bool triggerOnlyOnce = true;

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered && triggerOnlyOnce)
        {
            return;
        }

        if (other.CompareTag("Player"))
        {
            triggered = true;

            foreach (GameObject raptor in raptors)
            {
                if (raptor != null)
                {
                    raptor.SetActive(true);

                    
                    raptor.SendMessage("StartHunting", other.transform, SendMessageOptions.DontRequireReceiver);
                }
            }
        }
    }
}
