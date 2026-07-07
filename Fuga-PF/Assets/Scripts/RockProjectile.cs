using UnityEngine;

public class RockProjectile : MonoBehaviour
{
    [Header("Configuração")]
    [SerializeField] private float speed = 8f;
    [SerializeField] private int damage = 1;
    [SerializeField] private float lifeTime = 5f;

    private Rigidbody2D rb;
    private bool launched = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    public void Launch(Vector2 shootDirection)
    {
        Vector2 direction = shootDirection.normalized;

        launched = true;

        if (rb != null)
        {
            rb.linearVelocity = direction * speed;
        }

        if (direction.x < 0)
        {
            transform.localScale = new Vector3(
                -Mathf.Abs(transform.localScale.x),
                transform.localScale.y,
                transform.localScale.z
            );
        }
        else if (direction.x > 0)
        {
            transform.localScale = new Vector3(
                Mathf.Abs(transform.localScale.x),
                transform.localScale.y,
                transform.localScale.z
            );
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!launched)
            return;

        if (collision.CompareTag("Enemy"))
            return;

        if (collision.CompareTag("Player"))
        {
            Health playerHealth = collision.GetComponent<Health>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
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