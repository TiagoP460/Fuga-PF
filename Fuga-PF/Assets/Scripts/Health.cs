using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour
{
    [Header("Vida")]
    public int maxHealth = 5;
    public int currentHealth;

    [Header("Barra de vida")]
    public HealthBar healthBar;

    [Header("Animações")]
    public bool useDamageAnimation = true;
    public string damageTriggerName = "Damage";
    public string deathTriggerName = "Die";

    [Header("Morte")]
    public bool destroyOnDeath = true;
    public GameObject dropItem;
    public float deathDelay = 1.2f;

    [Header("Congelar ao morrer")]
    public bool freezeOnDeath = true;
    public bool disableColliderOnDeath = true;

    [Header("Reiniciar Cena ao Morrer")]
    public bool restartSceneOnDeath = false;
    public float restartDelay = 1.2f;

    private bool isDead = false;
    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();

        currentHealth = maxHealth;

        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
            healthBar.SetHealth(currentHealth);
        }

        Time.timeScale = 1f;
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
            PlayDamageAnimation();
        }
    }

    private void PlayDamageAnimation()
    {
        if (!useDamageAnimation) return;
        if (anim == null) return;

        if (HasAnimatorParameter(damageTriggerName, AnimatorControllerParameterType.Trigger))
        {
            anim.SetTrigger(damageTriggerName);
        }
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;

        if (dropItem != null)
        {
            Instantiate(dropItem, transform.position, Quaternion.identity);
        }

        PlayerMovement playerMovement = GetComponent<PlayerMovement>();

        if (playerMovement != null)
        {
            playerMovement.SetDead();
        }
        else
        {
            PlayDeathAnimation();
        }

        DisableEnemyAI();
        FreezeObject();

        if (restartSceneOnDeath)
        {
            StartCoroutine(RestartSceneRoutine());
            return;
        }

        StartCoroutine(DeathRoutine());
    }

    private void PlayDeathAnimation()
    {
        if (anim == null) return;

        if (HasAnimatorParameter("Walking", AnimatorControllerParameterType.Bool))
        {
            anim.SetBool("Walking", false);
        }

        if (HasAnimatorParameter("PlayerDetected", AnimatorControllerParameterType.Bool))
        {
            anim.SetBool("PlayerDetected", false);
        }

        if (HasAnimatorParameter("Throw", AnimatorControllerParameterType.Trigger))
        {
            anim.ResetTrigger("Throw");
        }

        if (HasAnimatorParameter("Damage", AnimatorControllerParameterType.Trigger))
        {
            anim.ResetTrigger("Damage");
        }

        if (HasAnimatorParameter(deathTriggerName, AnimatorControllerParameterType.Trigger))
        {
            anim.SetTrigger(deathTriggerName);
        }
    }

    private void DisableEnemyAI()
    {
        LizardBoss lizardBoss = GetComponent<LizardBoss>();

        if (lizardBoss != null)
        {
            lizardBoss.enabled = false;
        }
    }

    private void FreezeObject()
    {
        if (freezeOnDeath)
        {
            Rigidbody2D rb = GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
                rb.gravityScale = 0f;
                rb.constraints = RigidbodyConstraints2D.FreezeAll;
            }
        }

        if (disableColliderOnDeath)
        {
            Collider2D col = GetComponent<Collider2D>();

            if (col != null)
            {
                col.enabled = false;
            }
        }
    }

    private IEnumerator DeathRoutine()
    {
        yield return new WaitForSeconds(deathDelay);

        if (destroyOnDeath)
        {
            Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private IEnumerator RestartSceneRoutine()
    {
        yield return new WaitForSeconds(restartDelay);

        Time.timeScale = 1f;

        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    private bool HasAnimatorParameter(string parameterName, AnimatorControllerParameterType type)
    {
        if (anim == null) return false;

        foreach (AnimatorControllerParameter parameter in anim.parameters)
        {
            if (parameter.name == parameterName && parameter.type == type)
            {
                return true;
            }
        }

        return false;
    }
}