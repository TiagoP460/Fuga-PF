using UnityEngine;

public class GuardBullet : MonoBehaviour
{
    [Header("Configuração")]
    public float speed = 8f;
    public int damage = 1;
    public float lifeTime = 3f;

    [Header("Rotação visual")]
    public bool rotateBulletSprite = true;

    private int direction = -1;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void FixedUpdate()
    {
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(direction * speed, 0f);
        }
    }

    public void SetDirection(int newDirection)
    {
        direction = newDirection;

        if (rotateBulletSprite)
        {
            if (direction > 0)
            {
                transform.rotation = Quaternion.Euler(0f, 0f, -90f);
            }
            else
            {
                transform.rotation = Quaternion.Euler(0f, 0f, 90f);
            }
        }
        else
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * direction;
            transform.localScale = scale;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
            return;

        if (collision.CompareTag("Player"))
        {
            Health playerHealth = collision.GetComponent<Health>();

            if (playerHealth == null)
            {
                playerHealth = collision.GetComponentInParent<Health>();
            }

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                Debug.Log("Bala do guarda acertou o Player.");
            }
            else
            {
                Debug.LogWarning("O Player foi atingido, mas não tem Health.");
            }

            Destroy(gameObject);
            return;
        }

        if (!collision.isTrigger)
        {
            Destroy(gameObject);
        }
    }
}