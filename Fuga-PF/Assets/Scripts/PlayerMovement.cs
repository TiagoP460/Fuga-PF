using UnityEngine;

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

    private Rigidbody2D rb;
    private Animator anim;
    private PlayerInventory inventory;

    private float moveInput;
    private bool isGrounded;
    private bool facingRight = true;

    private float nextShootTime = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        inventory = GetComponent<PlayerInventory>();
    }

    void Update()
    {
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
        }
    }

    void Attack()
    {
        // Botão esquerdo do mouse
        if (Input.GetMouseButtonDown(0))
        {
            if (anim != null)
            {
                anim.SetTrigger("Knife");
            }

            if (attackPoint == null) return;

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
                }

                Health health = hit.GetComponent<Health>();

                if (health != null && hit.CompareTag("Enemy"))
                {
                    health.TakeDamage(attackDamage);
                }
            }
        }
    }

    void Shoot()
    {
        // Botão direito do mouse + cooldown
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

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        if (attackPoint != null)
        {
            Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
        }
    }
}