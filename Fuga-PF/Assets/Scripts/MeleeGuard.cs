using UnityEngine;

public class MeleeGuard : MonoBehaviour
{
    [Header("Alvo")]
    public Transform player;

    [Header("Ataque")]
    public float attackRange = 1.2f;
    public int damage = 1;
    public float attackCooldown = 1f;

    [Header("Movimento")]
    public float chaseRange = 5f;
    public float speed = 2.5f;

    private float nextAttackTime;
    private bool facingRight = false;

    void Update()
    {
        if (player == null)
        {
            Debug.LogWarning("Player não foi colocado no MeleeGuard.");
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            FacePlayer();
            Attack();
        }
        else if (distanceToPlayer <= chaseRange)
        {
            FacePlayer();
            ChasePlayer();
        }
    }

    void ChasePlayer()
    {
        Vector2 targetPosition = new Vector2(player.position.x, transform.position.y);

        transform.position = Vector2.MoveTowards(
            transform.position,
            targetPosition,
            speed * Time.deltaTime
        );
    }

    void Attack()
    {
        if (Time.time >= nextAttackTime)
        {
            Health playerHealth = player.GetComponent<Health>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                Debug.Log("Guarda bateu no Player!");
            }
            else
            {
                Debug.LogWarning("O Player não tem o script Health.");
            }

            nextAttackTime = Time.time + attackCooldown;
        }
    }

    void FacePlayer()
    {
        if (player.position.x > transform.position.x && !facingRight)
        {
            Flip();
        }
        else if (player.position.x < transform.position.x && facingRight)
        {
            Flip();
        }
    }

    void Flip()
    {
        facingRight = !facingRight;

        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}