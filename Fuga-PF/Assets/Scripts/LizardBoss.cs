using UnityEngine;

public class LizardBoss : MonoBehaviour
{
    [Header("Patrulha")]
    public Transform leftPoint;
    public Transform rightPoint;
    public float patrolSpeed = 2f;

    [Header("Player")]
    public Transform player;
    public float detectionRange = 8f;
    public float fieldOfViewDistance = 8f;
    public LayerMask obstacleLayer;

    [Header("Ataque")]
    public GameObject rockPrefab;
    public Transform throwPoint;
    public float attackCooldown = 2f;

    [Header("Som do arremesso")]
    public AudioClip throwRockSound;

    [Range(0f, 1f)]
    public float throwRockVolume = 1f;

    [Header("Animação")]
    public Animator animator;

    private Rigidbody2D rb;
    private AudioSource audioSource;

    private bool movingRight = true;
    private float attackTimer = 0f;
    private bool isDead = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 0f;

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
        if (isDead)
            return;

        attackTimer -= Time.deltaTime;

        if (player == null)
        {
            Patrol();
            return;
        }

        bool playerDetected = CanSeePlayer();

        if (animator != null)
        {
            animator.SetBool("PlayerDetected", playerDetected);
        }

        if (playerDetected)
        {
            StopMoving();
            FacePlayer();
            TryAttack();
        }
        else
        {
            Patrol();
        }
    }

    private void Patrol()
    {
        if (leftPoint == null || rightPoint == null)
        {
            StopMoving();
            return;
        }

        if (movingRight)
        {
            Move(Vector2.right);

            if (transform.position.x >= rightPoint.position.x)
            {
                movingRight = false;
                Flip();
            }
        }
        else
        {
            Move(Vector2.left);

            if (transform.position.x <= leftPoint.position.x)
            {
                movingRight = true;
                Flip();
            }
        }

        if (animator != null)
        {
            animator.SetBool("Walking", true);
        }
    }

    private void Move(Vector2 direction)
    {
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(direction.x * patrolSpeed, rb.linearVelocity.y);
        }
    }

    private void StopMoving()
    {
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }

        if (animator != null)
        {
            animator.SetBool("Walking", false);
        }
    }

    private void TryAttack()
    {
        if (attackTimer > 0f)
            return;

        attackTimer = attackCooldown;

        if (animator != null)
        {
            animator.ResetTrigger("Damage");
            animator.SetTrigger("Throw");
        }
    }

    public void ThrowRockFromAnimation()
    {
        ThrowRock();
    }

    private void ThrowRock()
    {
        if (rockPrefab == null || throwPoint == null || player == null)
        {
            Debug.LogWarning("Rock Prefab, Throw Point ou Player não foi configurado no LizardBoss.");
            return;
        }

        PlayThrowRockSound();

        GameObject rock = Instantiate(
            rockPrefab,
            throwPoint.position,
            Quaternion.identity
        );

        SpriteRenderer rockSprite = rock.GetComponent<SpriteRenderer>();

        if (rockSprite != null)
        {
            rockSprite.sortingOrder = 20;
        }

        RockProjectile rockProjectile = rock.GetComponent<RockProjectile>();

        if (rockProjectile != null)
        {
            Vector2 direction = player.position - throwPoint.position;
            rockProjectile.Launch(direction);
        }
    }

    private void PlayThrowRockSound()
    {
        if (audioSource != null && throwRockSound != null)
        {
            audioSource.PlayOneShot(throwRockSound, throwRockVolume);
        }
    }

    private bool CanSeePlayer()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer > detectionRange)
            return false;

        Vector2 directionToPlayer = player.position - transform.position;

        bool playerIsOnRight = directionToPlayer.x > 0f;
        bool lizardLookingRight = transform.localScale.x > 0f;

        if (playerIsOnRight != lizardLookingRight)
            return false;

        if (throwPoint == null)
            return true;

        Vector2 rayOrigin = throwPoint.position;
        Vector2 rayDirection = directionToPlayer.normalized;

        RaycastHit2D hit = Physics2D.Raycast(
            rayOrigin,
            rayDirection,
            fieldOfViewDistance,
            obstacleLayer
        );

        if (hit.collider != null)
        {
            return false;
        }

        return true;
    }

    private void FacePlayer()
    {
        if (player == null) return;

        bool playerIsOnRight = player.position.x > transform.position.x;
        bool lizardLookingRight = transform.localScale.x > 0f;

        if (playerIsOnRight != lizardLookingRight)
        {
            Flip();
        }
    }

    private void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1f;
        transform.localScale = scale;
    }

    public void SetDead()
    {
        isDead = true;
        StopMoving();

        if (animator != null)
        {
            animator.SetBool("Walking", false);
            animator.SetBool("PlayerDetected", false);
            animator.ResetTrigger("Throw");
            animator.ResetTrigger("Damage");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        if (throwPoint != null)
        {
            Gizmos.color = Color.red;
            Vector3 direction = transform.localScale.x > 0f ? Vector3.right : Vector3.left;
            Gizmos.DrawLine(throwPoint.position, throwPoint.position + direction * fieldOfViewDistance);
        }
    }
}