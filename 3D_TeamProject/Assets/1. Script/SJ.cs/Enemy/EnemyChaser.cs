
using UnityEngine;

public class EnemyChaser : MonoBehaviour
{
    [Header("애니메이션")]
    public Animator animator;

    public float moveSpeed = 2f;
    public float detectRange = 15f;
    public float attackRange = 1.5f;
    public int damage = 1;
    public float attackCooldown = 1f;

    [Header("몬스터 회전")]
    public float rotationSpeed = 10f;

    [Header("모델 방향 보정")]
    public Transform monsterModel;
    public float modelRotationY = 180f;

    Transform player;
    float lastAttackTime = -999f;

    void Start()
    {
        GameObject p = GameObject.FindWithTag("Player");

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (p != null)
            player = p.transform;

        // 자신의 모든 직속 자식 오브젝트 회전
        foreach (Transform child in transform)
        {
            child.localRotation *= Quaternion.Euler(0f, 180f, 0f);
        }
    }

    void Update()
    {
        if (player == null) return;

        Vector3 toPlayer = player.position - transform.position;
        toPlayer.y = 0f;

        float distance = toPlayer.magnitude;

        if (distance > detectRange) return;
        if (distance <= 0.001f) return;

        Vector3 direction = toPlayer.normalized;

        // 플레이어를 향해 회전
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );

        // 회전과 관계없이 플레이어 방향으로 이동
        if (distance > attackRange)
        {
            // 이동 중
            animator.SetFloat("Attack", 0f);

            transform.position +=
                direction * moveSpeed * Time.deltaTime;
        }
        else
        {
            // 공격 범위 안에 들어오면
            animator.SetFloat("Attack", 1f);

            if (Time.time >= lastAttackTime + attackCooldown)
            {
                lastAttackTime = Time.time;

                player.GetComponent<PayloadController>().TakeDamage(damage);
            }
        }
    }
}
