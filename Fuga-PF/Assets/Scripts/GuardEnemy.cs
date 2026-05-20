using UnityEngine;

public class GuardEnemy : MonoBehaviour
{
    [Header("Vida")]
    public int health = 3;

    [Header("Movimento")]
    public bool canPatrol = true;
    public float speed = 2f;
    public Transform pointA;
    public Transform pointB;

    [Header("Drop")]
    public GameObject itemDrop;

    private Transform currentTarget;
    private bool isDead = false;

    void Start()
    {
        currentTarget = pointB;
    }

    void Update()
    {
        if (isDead) return;

        if (canPatrol && pointA != null && pointB != null)
        {
            Patrol();
        }
    }

    void Patrol()
    {
        transform.position = Vector2.MoveTowards(
            transform.position,
            currentTarget.position,
            speed * Time.deltaTime
        );

        if (Vector2.Distance(transform.position, currentTarget.position) < 0.1f)
        {
            if (currentTarget == pointA)
                currentTarget = pointB;
            else
                currentTarget = pointA;

            Flip();
        }
    }

    void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        health -= damage;

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;

        if (itemDrop != null)
        {
            Instantiate(itemDrop, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}