using System.Collections;
using UnityEngine;

public class CollapsingFloor : MonoBehaviour
{
    [Header("Floor Parts")]
    public GameObject floorObject;

    [Header("Timing")]
    public float collapseDelay = 0.6f;
    public float respawnDelay = 3f;

    [Header("Optional Warning")]
    public bool shakeBeforeCollapse = true;
    public float shakeAmount = 0.04f;

    private Renderer[] floorRenderers;
    private Collider[] floorColliders;
    private bool isCollapsing = false;
    private Vector3 originalPosition;

    void Start()
    {
        if (floorObject == null)
        {
            Debug.LogError("CollapsingFloor: floorObject is not assigned.");
            return;
        }

        floorRenderers = floorObject.GetComponentsInChildren<Renderer>();
        floorColliders = floorObject.GetComponentsInChildren<Collider>();

        originalPosition = floorObject.transform.position;
    }

    void OnTriggerEnter(Collider other)
    {
        if (isCollapsing) return;

        if (other.CompareTag("Player"))
        {
            StartCoroutine(CollapseRoutine());
        }
    }

    IEnumerator CollapseRoutine()
    {
        isCollapsing = true;

        float timer = 0f;

        while (timer < collapseDelay)
        {
            if (shakeBeforeCollapse)
            {
                Vector3 randomOffset = new Vector3(
                    Random.Range(-shakeAmount, shakeAmount),
                    0f,
                    Random.Range(-shakeAmount, shakeAmount)
                );

                floorObject.transform.position = originalPosition + randomOffset;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        floorObject.transform.position = originalPosition;

        SetFloorActive(false);

        yield return new WaitForSeconds(respawnDelay);

        SetFloorActive(true);

        isCollapsing = false;
    }

    void SetFloorActive(bool active)
    {
        foreach (Renderer r in floorRenderers)
        {
            r.enabled = active;
        }

        foreach (Collider c in floorColliders)
        {
            c.enabled = active;
        }
    }
}
