using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movimento")]
    public float speed = 5f;
    public float jumpForce = 8f;

    [Header("Verificação de chão")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Ataque corpo a corpo")]
    public Transform attackPoint;
    public float attackRadius = 0.5f;
    public LayerMask attackLayer;
    public int attackDamage = 1;

    [Header("Tiro")]
    public Transform firePoint;
    public GameObject bulletPrefab;
    public float shootCooldown = 0.6f;

    [Header("Sons")]
    public AudioClip shootSound;
    [Range(0f, 1f)] public float shootVolume = 1f;

    public AudioClip knifeSound;
    [Range(0f, 1f)] public float knifeVolume = 1f;

    private Rigidbody2D rb;
    private Animator anim;
    private PlayerInventory inventory;
    private AudioSource audioSource;

    private float moveInput;
    private bool isGrounded;
    private bool facingRight = true;

    private float nextShootTime = 0f;
    private bool isDead = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        inventory = GetComponent<PlayerInventory>();
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 0f;
    }

    void Update()
    {
        if (isDead)
            return;

        moveInput = Input.GetAxisRaw("Horizontal");

        CheckGround();
        Jump();
        Attack();
        Shoot();
        FlipCharacter();
        UpdateAnimations();
    }

    void FixedUpdate()
    {
        if (isDead)
            return;

        rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y);
    }

    void CheckGround()
    {
        if (groundCheck == null) return;

        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

            if (anim != null)
            {
                anim.SetTrigger("Jump");
            }
        }
    }

    void Attack()
    {
        if (IsPointerOverUI())
            return;

        if (Input.GetMouseButtonDown(0))
        {
            if (anim != null)
            {
                anim.SetTrigger("Knife");
            }

            PlayKnifeSound();

            if (attackPoint == null)
            {
                Debug.LogWarning("AttackPoint não foi configurado no Player.");
                return;
            }

            Collider2D[] hitObjects = Physics2D.OverlapCircleAll(
                attackPoint.position,
                attackRadius,
                attackLayer
            );

            foreach (Collider2D hit in hitObjects)
            {
                DestructibleCrate crate = hit.GetComponent<DestructibleCrate>();

                if (crate != null)
                {
                    crate.TakeDamage(attackDamage);
                    continue;
                }

                GuardHealth guardHealth = hit.GetComponent<GuardHealth>();

                if (guardHealth == null)
                {
                    guardHealth = hit.GetComponentInParent<GuardHealth>();
                }

                if (guardHealth != null)
                {
                    guardHealth.TakeDamage(attackDamage);
                    Debug.Log("Player acertou o guarda com ataque corpo a corpo.");
                    continue;
                }

                Health health = hit.GetComponent<Health>();

                if (health == null)
                {
                    health = hit.GetComponentInParent<Health>();
                }

                if (health != null && hit.CompareTag("Enemy"))
                {
                    health.TakeDamage(attackDamage);
                    Debug.Log("Player acertou inimigo com Health.");
                }
            }
        }
    }

    void Shoot()
    {
        if (IsPointerOverUI())
            return;

        if (Input.GetMouseButtonDown(1) && Time.time >= nextShootTime)
        {
            if (inventory == null || !inventory.hasGun)
            {
                if (MessageManager.instance != null)
                {
                    MessageManager.instance.ShowMessage("Você ainda não tem uma arma.");
                }

                return;
            }

            if (bulletPrefab == null || firePoint == null)
            {
                Debug.LogWarning("FirePoint ou BulletPrefab não foi configurado no PlayerMovement.");
                return;
            }

            if (anim != null)
            {
                anim.SetTrigger("Shoot");
            }

            PlayShootSound();

            GameObject bullet = Instantiate(
                bulletPrefab,
                firePoint.position,
                Quaternion.identity
            );

            PlayerBullet bulletScript = bullet.GetComponent<PlayerBullet>();

            if (bulletScript != null)
            {
                int direction = facingRight ? 1 : -1;
                bulletScript.SetDirection(direction);
            }

            nextShootTime = Time.time + shootCooldown;
        }
    }

    void PlayShootSound()
    {
        if (audioSource != null && shootSound != null)
        {
            audioSource.PlayOneShot(shootSound, shootVolume);
        }
    }

    void PlayKnifeSound()
    {
        if (audioSource != null && knifeSound != null)
        {
            audioSource.PlayOneShot(knifeSound, knifeVolume);
        }
    }

    bool IsPointerOverUI()
    {
        if (EventSystem.current == null)
            return false;

        return EventSystem.current.IsPointerOverGameObject();
    }

    void FlipCharacter()
    {
        if (moveInput > 0 && !facingRight)
        {
            Flip();
        }
        else if (moveInput < 0 && facingRight)
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

    void UpdateAnimations()
    {
        if (anim == null) return;

        anim.SetFloat("Speed", Mathf.Abs(moveInput));
        anim.SetBool("IsGrounded", isGrounded);
    }

    public void SetDead()
    {
        isDead = true;
        moveInput = 0f;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }

        if (anim != null)
        {
            anim.SetFloat("Speed", 0f);
            anim.SetTrigger("Die");
        }
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
        }
    }
}