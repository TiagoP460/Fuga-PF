using System.Collections;
using UnityEngine;

public class GuardEnemy : MonoBehaviour
{
    [Header("Vida")]
    public int maxHealth = 3;
    public int currentHealth;

    [Header("Barra de vida")]
    public HealthBar healthBar;

    [Header("Drop")]
    public GameObject itemDrop;

    [Header("Morte")]
    public float deathDelay = 0.9f;

    private bool isDead = false;
    private Animator anim;
    private Collider2D col;

    private void Start()
    {
        anim = GetComponent<Animator>();
        col = GetComponent<Collider2D>();

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
            if (anim != null)
            {
                anim.ResetTrigger("Shoot");
                anim.SetTrigger("Damage");
            }
        }
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;

        if (anim != null)
        {
            anim.SetFloat("Speed", 0f);
            anim.ResetTrigger("Shoot");
            anim.ResetTrigger("Damage");
            anim.SetTrigger("Death");
        }

        if (col != null)
        {
            col.enabled = false;
        }

        if (itemDrop != null)
        {
            Instantiate(itemDrop, transform.position, Quaternion.identity);
        }

        StartCoroutine(DeathRoutine());
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