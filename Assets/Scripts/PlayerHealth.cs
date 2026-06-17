using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 3;
    public int currentHealth;

    [Header("References")]
    public LevelManager levelManager;

    [Header("Heart UI")]
    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    [Header("Damage Sound")]
    public AudioSource audioSource;
    public AudioClip hurtSound;
    public float hurtSoundVolume = 1f;

    [Header("Invincible")]
    public float invincibleTime = 3f;
    public float blinkInterval = 0.15f;

    [Header("Fall Death")]
    public bool enableFallDeath = true;
    public float fallDeathY = -10f;

    private bool isDead = false;

    private bool isInvincible = false;
    private Renderer[] renderers;
    private bool[] originalRendererStates;

    void Start()
    {
        currentHealth = maxHealth;

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        renderers = GetComponentsInChildren<Renderer>();
        originalRendererStates = new bool[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            originalRendererStates[i] = renderers[i].enabled;
        }

        UpdateHeartUI();
    }

    void Update()
    {
        if (!enableFallDeath)
        {
            return;
        }

        if (isDead)
        {
            return;
        }

        if (transform.position.y < fallDeathY)
        {
            Die();
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead)
        {
            return;
        }

        if (isInvincible)
        {
            return;
        }

        PlayerShield shield = GetComponent<PlayerShield>();

        if (shield != null && shield.UseShield())
        {
            StartCoroutine(InvincibleRoutine());
            return;
        }

        currentHealth -= damage;

        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        PlayHurtSound();

        UpdateHeartUI();

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(InvincibleRoutine());
        }
    }

    private void PlayHurtSound()
    {
        if (hurtSound == null)
        {
            return;
        }

        if (audioSource != null)
        {
            audioSource.PlayOneShot(hurtSound, hurtSoundVolume);
        }
        else
        {
            AudioSource.PlayClipAtPoint(hurtSound, transform.position, hurtSoundVolume);
        }
    }

    private void Die()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;

        Debug.Log("Game Over!");

        currentHealth = 0;
        UpdateHeartUI();

        RestoreOriginalRenderers();

        if (levelManager != null)
        {
            levelManager.GameOver();
        }
    }

    private IEnumerator InvincibleRoutine()
    {
        isInvincible = true;

        float timer = 0f;

        while (timer < invincibleTime)
        {
            SetVisibleRenderers(false);
            yield return new WaitForSeconds(blinkInterval);

            SetVisibleRenderers(true);
            yield return new WaitForSeconds(blinkInterval);

            timer += blinkInterval * 2f;
        }

        RestoreOriginalRenderers();
        isInvincible = false;
    }

    private void SetVisibleRenderers(bool value)
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] != null && originalRendererStates[i])
            {
                renderers[i].enabled = value;
            }
        }
    }

    private void RestoreOriginalRenderers()
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] != null)
            {
                renderers[i].enabled = originalRendererStates[i];
            }
        }
    }

    private void UpdateHeartUI()
    {
        if (hearts == null || hearts.Length == 0)
        {
            return;
        }

        for (int i = 0; i < hearts.Length; i++)
        {
            if (hearts[i] == null)
            {
                continue;
            }

            if (i < currentHealth)
            {
                hearts[i].sprite = fullHeart;
            }
            else
            {
                hearts[i].sprite = emptyHeart;
            }

            hearts[i].enabled = true;
        }
    }
}
