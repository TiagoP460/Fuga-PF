using System.Collections;
using UnityEngine;

public class GuardHealth : MonoBehaviour
{
    [Header("Vida")]
    public int maxHealth = 3;
    public int currentHealth;

    [Header("Barra de vida")]
    public HealthBar healthBar;

    [Header("Drop")]
    public GameObject dropItem;

    [Header("Morte")]
    public float deathDelay = 1f;

    private bool isDead = false;
    private Animator animator;
    private Collider2D col;

    private MonoBehaviour[] behaviours;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
        behaviours = GetComponents<MonoBehaviour>();
    }

    private void Start()
    {
        currentHealth = maxHealth;

        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
            healthBar.SetHealth(currentHealth);
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            if (animator != null)
            {
                animator.ResetTrigger("Shoot");
                animator.SetTrigger("Damage");
            }
        }
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;

        if (animator != null)
        {
            animator.SetFloat("Speed", 0f);
            animator.ResetTrigger("Shoot");
            animator.ResetTrigger("Damage");
            animator.SetTrigger("Death");
        }

        if (dropItem != null)
        {
            Instantiate(dropItem, transform.position, Quaternion.identity);
        }

        if (col != null)
        {
            col.enabled = false;
        }

        DisableEnemyScripts();

        StartCoroutine(DeathRoutine());
    }

    private void DisableEnemyScripts()
    {
        foreach (MonoBehaviour behaviour in behaviours)
        {
            if (behaviour == this) continue;

            if (behaviour is GuardShooterAI || behaviour is GuardMeleeAI)
            {
                behaviour.enabled = false;
            }
        }
    }

    private IEnumerator DeathRoutine()
    {
        yield return new WaitForSeconds(deathDelay);
        Destroy(gameObject);
    }

    public bool IsDead()
    {
        return isDead;
    }
}