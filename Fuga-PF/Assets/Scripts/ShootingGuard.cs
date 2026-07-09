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

    [Header("Direção inicial")]
    [SerializeField] private bool facingRight = true;

    private Transform currentTarget;
    private float nextShootTime;

    private Animator anim;
    private GuardEnemy guardEnemy;
    private float startY;

    private void Start()
    {
        currentTarget = pointB;
        anim = GetComponent<Animator>();
        guardEnemy = GetComponent<GuardEnemy>();

        startY = transform.position.y;

        ApplyFacingDirection();
    }

    private void Update()
    {
        if (guardEnemy != null && guardEnemy.IsDead())
        {
            SetSpeed(0f);
            return;
        }

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
        Vector2 oldPosition = transform.position;

        Vector2 targetPosition = new Vector2(
            currentTarget.position.x,
            startY
        );

        transform.position = Vector2.MoveTowards(
            transform.position,
            targetPosition,
            speed * Time.deltaTime
        );

        transform.position = new Vector3(
            transform.position.x,
            startY,
            transform.position.z
        );

        float moved = Vector2.Distance(oldPosition, transform.position);
        SetSpeed(moved > 0.001f ? speed : 0f);

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

        if (anim != null)
        {
            anim.SetTrigger("Shoot");
        }

        Shoot();
    }

    private void Shoot()
    {
        if (bulletPrefab == null || firePoint == null)
            return;

        GameObject bullet = Instantiate(
            bulletPrefab,
            firePoint.position,
            Quaternion.identity
        );

        EnemyBullet enemyBullet = bullet.GetComponent<EnemyBullet>();

        if (enemyBullet != null)
        {
            int direction = facingRight ? 1 : -1;
            enemyBullet.SetDirection(direction);
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
        if (anim != null)
        {
            anim.SetFloat("Speed", Mathf.Abs(value));
        }
    }
}