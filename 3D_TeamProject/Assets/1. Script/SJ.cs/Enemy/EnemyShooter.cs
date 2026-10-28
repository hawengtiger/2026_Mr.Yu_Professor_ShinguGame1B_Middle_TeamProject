
using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    [Header("총알 포물선")]
    public float bulletUpForce = 6f;

    public GameObject bulletPrefab;
    public Transform firePoint;
    public float moveSpeed = 2f;
    public float keepDistance = 8f;
    public float fireRange = 12f;
    public float fireInterval = 2f;
    public float bulletSpeed = 10f;

    [Header("애니메이션")]
    public Animator animator;
    public float attackAnimationDuration = 0.5f;

    Transform player;
    float lastFireTime;
    float attackEndTime;

    void Start()
    {
        foreach (Transform child in transform)
        {
            child.localRotation *= Quaternion.Euler(0f, 180f, 0f);
        }

        GameObject p = GameObject.FindWithTag("Player");

        if (p != null)
            player = p.transform;

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        lastFireTime = Time.time;
    }

    void Update()
    {
        if (player == null) return;

        Vector3 toPlayer = player.position - transform.position;
        toPlayer.y = 0f;

        float distance = toPlayer.magnitude;

        if (distance > 0.001f)
            transform.forward = toPlayer.normalized;

        // 공격 애니메이션 유지 시간
        if (animator != null && Time.time >= attackEndTime)
        {
            animator.SetFloat("Attack", 0f);
        }

        // 플레이어에게 접근
        if (distance > keepDistance)
        {
            float move = moveSpeed * Time.deltaTime;
            transform.position += transform.forward * move;
        }

        bool inRange = distance < fireRange;
        bool ready = Time.time >= lastFireTime + fireInterval;

        if (inRange && ready)
        {
            lastFireTime = Time.time;
            Shoot();
        }
    }

    void Shoot()
    {
        if (bulletPrefab == null || firePoint == null)
            return;

        // 공격 애니메이션 활성화
        if (animator != null)
        {
            animator.SetFloat("Attack", 1f);
            attackEndTime = Time.time + attackAnimationDuration;
        }

        Vector3 target = player.position + Vector3.up * 2f;
        Vector3 dir = (target - firePoint.position).normalized;

        GameObject bullet = Instantiate(
            bulletPrefab,
            firePoint.position,
            Quaternion.LookRotation(dir)
        );

        Rigidbody rb = bullet.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity =
                dir * bulletSpeed +
                Vector3.up * bulletUpForce;
        }
    }
}
