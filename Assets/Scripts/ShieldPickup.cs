using System.Collections;
using UnityEngine;

public class ShieldPickup : MonoBehaviour
{
    [Header("Pickup Sound")]
    public AudioClip pickupSound;
    public float pickupSoundVolume = 1f;

    [Header("Animation")]
    public Animator animator;
    public float destroyDelay = 0.35f;

    private bool collected = false;
    private Collider itemCollider;

    void Start()
    {
        itemCollider = GetComponent<Collider>();

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (collected)
        {
            return;
        }

        if (!other.CompareTag("Player"))
        {
            return;
        }

        PlayerShield playerShield = other.GetComponent<PlayerShield>();

        if (playerShield == null)
        {
            playerShield = other.GetComponentInParent<PlayerShield>();
        }

        if (playerShield == null)
        {
            playerShield = other.GetComponentInChildren<PlayerShield>();
        }

        if (playerShield == null)
        {
            Debug.LogWarning("ShieldPickup: PlayerShield not found on Player.");
            return;
        }

        collected = true;

        playerShield.AddShield();

        if (itemCollider != null)
        {
            itemCollider.enabled = false;
        }

        if (pickupSound != null)
        {
            AudioSource.PlayClipAtPoint(pickupSound, transform.position, pickupSoundVolume);
        }

        if (animator != null)
        {
            animator.SetTrigger("Pickup");
        }

        StartCoroutine(DestroyAfterAnimation());
    }

    IEnumerator DestroyAfterAnimation()
    {
        yield return new WaitForSeconds(destroyDelay);
        Destroy(gameObject);
    }
}
