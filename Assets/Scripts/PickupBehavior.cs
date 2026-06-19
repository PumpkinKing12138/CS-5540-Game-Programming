using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UIElements;

public class PickupBehavior : MonoBehaviour
{
    public GameObject heart;
    public float rotationSpeed = 30;
    public AudioClip pickupSFX;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Rotate();
    }

    void Rotate()
    {
        transform.Rotate(Vector3.left * rotationSpeed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider collider)
    {
        Debug.Log("Trigger");
        if (collider.CompareTag("Player"))
        {
            Debug.Log("Player");
            DestroyPickup();
        }
    }

    void DestroyPickup()
    {
        
        PlayAudioEffect();

        // Animator animator = GetComponent<Animator>();
        // animator.SetTrigger("pickupDestroy");

        heart.GetComponent<DinoHeartTrigger>().getKey = true;
        Destroy(gameObject);
    }

    void PlayAudioEffect()
    {
        AudioSource.PlayClipAtPoint(pickupSFX, gameObject.transform.position);
        // var audioSource = GetComponent<AudioSource>();
        // audioSource.Play();
    }
}
