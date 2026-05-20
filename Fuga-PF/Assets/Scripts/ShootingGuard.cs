using UnityEngine;

public class ShootingGuard : MonoBehaviour
{
    [Header("Alvo")]
    public Transform player;

    [Header("Tiro")]
    public Transform firePoint;
    public GameObject bulletPrefab;
    public float shootRange = 8f;
    public float shootCooldown = 1.5f;

    [Header("Patrulha")]
    public bool canPatrol = true;
    public float speed = 2f;
    public Transform pointA;
    public Transform pointB;

    private Transform currentTarget;
    private float nextShootTime;
    private bool facingRight = false;

    void Start()
    {
        currentTarget = pointB;
    }

    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= shootRange)
        {
            FacePlayer();
            TryShoot();
        }
        else if (canPatrol && pointA != null && pointB != null)
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
            currentTarget = currentTarget == pointA ? pointB : pointA;
            Flip();
        }
    }

    void TryShoot()
    {
        if (Time.time >= nextShootTime)
        {
            Shoot();
            nextShootTime = Time.time + shootCooldown;
        }
    }

    void Shoot()
    {
        if (bulletPrefab == null || firePoint == null) return;

        GameObject bullet = Instantiate(
            bulletPrefab,
            firePoint.position,
            Quaternion.identity
        );

        EnemyBullet enemyBullet = bullet.GetComponent<EnemyBullet>();

        if (enemyBullet != null)
        {
            int direction = player.position.x > transform.position.x ? 1 : -1;
            enemyBullet.SetDirection(direction);
        }
    }

    void FacePlayer()
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

    void Flip()
    {
        facingRight = !facingRight;

        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}