using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 1;
    public float lifeTime = 2f;

    private int direction = 1;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        transform.Translate(Vector2.right * direction * speed * Time.deltaTime);
    }

    public void SetDirection(int newDirection)
    {
        direction = newDirection;

        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * direction;
        transform.localScale = scale;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            return;

        GuardHealth guardHealth = collision.GetComponent<GuardHealth>();

        if (guardHealth == null)
        {
            guardHealth = collision.GetComponentInParent<GuardHealth>();
        }

        if (guardHealth != null)
        {
            guardHealth.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        Health health = collision.GetComponent<Health>();

        if (health == null)
        {
            health = collision.GetComponentInParent<Health>();
        }

        if (health != null && collision.CompareTag("Enemy"))
        {
            health.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        if (!collision.isTrigger)
        {
            Destroy(gameObject);
        }
    }
}