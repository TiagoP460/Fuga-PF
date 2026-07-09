using UnityEngine;

public class GuardShooterAI : MonoBehaviour
{
    [Header("Player")]
    public Transform player;

    [Header("Patrulha")]
    public bool canPatrol = true;
    public Transform pointA;
    public Transform pointB;
    public float speed = 2f;

    [Header("Tiro")]
    public Transform firePoint;
    public GameObject bulletPrefab;
    public float shootRange = 8f;
    public float shootCooldown = 1.5f;

    [Header("Som do tiro")]
    public AudioClip shootSound;

    [Range(0f, 1f)]
    public float shootVolume = 1f;

    [Header("Direção inicial")]
    public bool facingRight = true;

    private Transform currentTarget;
    private Animator animator;
    private GuardHealth guardHealth;
    private AudioSource audioSource;

    private float nextShootTime;
    private float fixedY;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        guardHealth = GetComponent<GuardHealth>();
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 0f;
    }

    private void Start()
    {
        currentTarget = pointB;
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

        if (distanceToPlayer <= shootRange)
        {
            SetSpeed(0f);
            FacePlayer();
            TryShoot();
        }
        else if (canPatrol && pointA != null && pointB != null)
        {
            Patrol();
        }
        else
        {
            SetSpeed(0f);
        }
    }

    private void Patrol()
    {
        Vector3 oldPosition = transform.position;

        Vector3 targetPosition = new Vector3(
            currentTarget.position.x,
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

        if (Mathf.Abs(transform.position.x - currentTarget.position.x) < 0.1f)
        {
            currentTarget = currentTarget == pointA ? pointB : pointA;
            Flip();
        }
    }

    private void TryShoot()
    {
        if (Time.time < nextShootTime)
            return;

        nextShootTime = Time.time + shootCooldown;

        if (animator != null)
        {
            animator.SetTrigger("Shoot");
        }

        Shoot();
    }

    private void Shoot()
    {
        if (bulletPrefab == null || firePoint == null)
            return;

        PlayShootSound();

        GameObject bullet = Instantiate(
            bulletPrefab,
            firePoint.position,
            Quaternion.identity
        );

        GuardBullet guardBullet = bullet.GetComponent<GuardBullet>();

        if (guardBullet != null)
        {
            int direction = facingRight ? 1 : -1;
            guardBullet.SetDirection(direction);
        }
    }

    private void PlayShootSound()
    {
        if (audioSource != null && shootSound != null)
        {
            audioSource.PlayOneShot(shootSound, shootVolume);
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