using UnityEngine;

public class GuardMeleeAI : MonoBehaviour
{
    [Header("Player")]
    public Transform player;

    [Header("Movimento")]
    public float chaseRange = 5f;
    public float speed = 2.5f;

    [Header("Ataque")]
    public float attackRange = 1.2f;
    public int damage = 1;
    public float attackCooldown = 1f;

    [Header("Direção inicial")]
    public bool facingRight = false;

    private Animator animator;
    private GuardHealth guardHealth;
    private float nextAttackTime;
    private float fixedY;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        guardHealth = GetComponent<GuardHealth>();
    }

    private void Start()
    {
        fixedY = transform.position.y;

        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }

        ApplyFacingDirection();
    }

    private void Update()
    {
        if (guardHealth != null && guardHealth.IsDead())
            return;

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
        Vector3 oldPosition = transform.position;

        Vector3 targetPosition = new Vector3(
            player.position.x,
            fixedY,
            transform.position.z
        );

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            speed * Time.deltaTime
        );

        transform.position = new Vector3(
            transform.position.x,
            fixedY,
            transform.position.z
        );

        float movementAmount = Vector3.Distance(oldPosition, transform.position);
        SetSpeed(movementAmount > 0.001f ? speed : 0f);
    }

    private void Attack()
    {
        if (Time.time < nextAttackTime)
            return;

        nextAttackTime = Time.time + attackCooldown;

        if (animator != null)
        {
            animator.SetTrigger("Shoot");
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
        if (animator != null)
        {
            animator.SetFloat("Speed", Mathf.Abs(value));
        }
    }
}