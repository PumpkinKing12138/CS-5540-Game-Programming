using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BaseBehavior : MonoBehaviour
{
    public int health = 100;
    public Slider healthSlider;
    public ParticleSystem baseAttackVfx;

    int maxHealth;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        maxHealth = health;
        if (healthSlider)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = health;
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (healthSlider)
        {
            healthSlider.value = health;
        }
        if (health <= 0)
        {
            Debug.Log("Game Over");
            health = 0;
            GameLost();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyAI enemyAI = other.gameObject.GetComponent<EnemyAI>();
            if (enemyAI)
            {
                int baseDamageValue = enemyAI.GetEnemyDamageValue();
                TakeDamage(baseDamageValue);
                if (baseAttackVfx)
                    baseAttackVfx.Play();
            }
            Destroy(other.gameObject);
        }
    }

    void GameLost()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
