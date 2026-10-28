using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    [Header("총알 포물선")]
    public float bulletUpForce = 6f;

    public GameObject bulletPrefab;     // EnemeyBullet
    public Transform firePoint;
    public float moveSpeed = 2f;
    public float keepDistance = 8f;        // 여기까지만 다가옴
    public float fireRange = 12f;            // 이 안이면 발사
    public float fireInterval = 2f;            // 총알 타이밍
    public float bulletSpeed = 10f;       // 총알 속도

    Transform player;
    float lastFireTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 자신의 모든 직속 자식 오브젝트 회전
        foreach (Transform child in transform)
        {
            child.localRotation *= Quaternion.Euler(0f, 180f, 0f);
        }

        GameObject p = GameObject.FindWithTag("Player");
        if (p != null) player = p.transform;
        lastFireTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null) return;

        Vector3 toPlayer = player.position - transform.position;
        toPlayer.y = 0;
        float distance = toPlayer.magnitude;
        transform.forward = toPlayer.normalized;

        if (distance > keepDistance)        // 다가가기
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
        Vector3 target = player.position + Vector3.up * 2f;
        Vector3 dir = (target - firePoint.position).normalized;

        GameObject bullet = Instantiate(
            bulletPrefab,
            firePoint.position,
            Quaternion.LookRotation(dir)
        );

        Rigidbody rb = bullet.GetComponent<Rigidbody>();

        // 플레이어 방향으로 발사 + 위쪽으로 힘 추가
        rb.linearVelocity =
            dir * bulletSpeed +
            Vector3.up * bulletUpForce;
    }
}
