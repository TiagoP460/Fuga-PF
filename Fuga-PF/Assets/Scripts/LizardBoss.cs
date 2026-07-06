using UnityEngine;

public class LizardBoss : MonoBehaviour
{
    [Header("Patrulha")]
    [SerializeField] private Transform leftPoint;
    [SerializeField] private Transform rightPoint;
    [SerializeField] private float patrolSpeed = 2f;

    [Header("Detecção")]
    [SerializeField] private Transform player;
    [SerializeField] private float detectionRange = 7f;
    [SerializeField] private float fieldOfViewDistance = 8f;
    [SerializeField] private LayerMask obstacleLayer;

    [Header("Ataque")]
    [SerializeField] private GameObject rockPrefab;
    [SerializeField] private Transform throwPoint;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private float rockShootForce = 8f;

    [Header("Animação")]
    [SerializeField] private Animator animator;

    private Rigidbody2D rb;
    private bool movingRight = true;
    private bool playerDetected = false;
    private float attackTimer = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    private void Start()
    {
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }
    }

    private void Update()
    {
        if (player == null)
            return;

        attackTimer -= Time.deltaTime;

        playerDetected = CanSeePlayer();

        if (playerDetected)
        {
            FacePlayer();
            StopMoving();
            AttackPlayer();
        }
        else
        {
            Patrol();
        }

        UpdateAnimations();
    }

    private void Patrol()
    {
        if (leftPoint == null || rightPoint == null)
            return;

        float direction = movingRight ? 1f : -1f;

        rb.linearVelocity = new Vector2(
            direction * patrolSpeed,
            rb.linearVelocity.y
        );

        if (movingRight && transform.position.x >= rightPoint.position.x)
        {
            movingRight = false;
            Flip(false);
        }
        else if (!movingRight && transform.position.x <= leftPoint.position.x)
        {
            movingRight = true;
            Flip(true);
        }
    }

    private bool CanSeePlayer()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer > detectionRange)
            return false;

        Vector2 directionToPlayer = player.position - transform.position;

        bool playerIsInFront;

        if (transform.localScale.x > 0)
        {
            playerIsInFront = directionToPlayer.x > 0;
        }
        else
        {
            playerIsInFront = directionToPlayer.x < 0;
        }

        if (!playerIsInFront)
            return false;

        RaycastHit2D hit = Physics2D.Raycast(
            throwPoint.position,
            directionToPlayer.normalized,
            fieldOfViewDistance,
            obstacleLayer
        );

        if (hit.collider != null)
        {
            return false;
        }

        return true;
    }

    private void AttackPlayer()
    {
        if (attackTimer > 0f)
            return;

        attackTimer = attackCooldown;

        if (animator != null)
        {
            animator.SetTrigger("Throw");
        }

        ThrowRock();
    }

    private void ThrowRock()
    {
        if (rockPrefab == null || throwPoint == null || player == null)
            return;

        GameObject rock = Instantiate(
            rockPrefab,
            throwPoint.position,
            Quaternion.identity
        );

        RockProjectile rockProjectile = rock.GetComponent<RockProjectile>();

        if (rockProjectile != null)
        {
            Vector2 direction = player.position - throwPoint.position;
            rockProjectile.Launch(direction);
        }
    }

    private void StopMoving()
    {
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
    }

    private void FacePlayer()
    {
        if (player.position.x > transform.position.x)
        {
            movingRight = true;
            Flip(true);
        }
        else
        {
            movingRight = false;
            Flip(false);
        }
    }

    private void Flip(bool faceRight)
    {
        if (faceRight)
        {
            transform.localScale = new Vector3(
                Mathf.Abs(transform.localScale.x),
                transform.localScale.y,
                transform.localScale.z
            );
        }
        else
        {
            transform.localScale = new Vector3(
                -Mathf.Abs(transform.localScale.x),
                transform.localScale.y,
                transform.localScale.z
            );
        }
    }

    private void UpdateAnimations()
    {
        if (animator == null)
            return;

        bool isWalking = Mathf.Abs(rb.linearVelocity.x) > 0.1f && !playerDetected;

        animator.SetBool("Walking", isWalking);
        animator.SetBool("PlayerDetected", playerDetected);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        if (throwPoint != null)
        {
            Gizmos.color = Color.red;

            Vector3 direction = transform.localScale.x > 0
                ? Vector3.right
                : Vector3.left;

            Gizmos.DrawLine(
                throwPoint.position,
                throwPoint.position + direction * fieldOfViewDistance
            );
        }
    }
}