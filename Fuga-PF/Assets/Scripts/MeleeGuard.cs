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

    [Header("Direção inicial")]
    [SerializeField] private bool facingRight = false;

    private float nextAttackTime;

    private Animator anim;
    private GuardEnemy guardEnemy;
    private float startY;

    private void Start()
    {
        anim = GetComponent<Animator>();
        guardEnemy = GetComponent<GuardEnemy>();

        startY = transform.position.y;

        ApplyFacingDirection();
    }

    private void Update()
    {
        if (guardEnemy != null && guardEnemy.IsDead())
        {
            SetSpeed(0f);
            return;
        }

        if (player == null)
        {
            SetSpeed(0f);
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            SetSpeed(0f);
            FacePlayer();
            Attack();
        }
        else if (distanceToPlayer <= chaseRange)
        {
            FacePlayer();
            ChasePlayer();
        }
        else
        {
            SetSpeed(0f);
        }
    }

    private void ChasePlayer()
    {
        Vector2 oldPosition = transform.position;

        Vector2 targetPosition = new Vector2(
            player.position.x,
            startY
        );

        transform.position = Vector2.MoveTowards(
            transform.position,
            targetPosition,
            speed * Time.deltaTime
        );

        transform.position = new Vector3(
            transform.position.x,
            startY,
            transform.position.z
        );

        float moved = Vector2.Distance(oldPosition, transform.position);
        SetSpeed(moved > 0.001f ? speed : 0f);
    }

    private void Attack()
    {
        if (Time.time < nextAttackTime)
            return;

        nextAttackTime = Time.time + attackCooldown;

        if (anim != null)
        {
            anim.SetTrigger("Shoot");
        }

        Health playerHealth = player.GetComponent<Health>();

        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
        }
    }

    private void FacePlayer()
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

    private void Flip()
    {
        facingRight = !facingRight;
        ApplyFacingDirection();
    }

    private void ApplyFacingDirection()
    {
        Vector3 scale = transform.localScale;

        if (facingRight)
        {
            scale.x = Mathf.Abs(scale.x);
        }
        else
        {
            scale.x = -Mathf.Abs(scale.x);
        }

        transform.localScale = scale;
    }

    private void SetSpeed(float value)
    {
        if (anim != null)
        {
            anim.SetFloat("Speed", Mathf.Abs(value));
        }
    }
}