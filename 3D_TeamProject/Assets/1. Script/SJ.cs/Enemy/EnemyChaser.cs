using UnityEngine;

public class EnemyChaser : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float detectRange = 15f;     // 이 안에 있으면 이동
    public float attackRange = 1.5f;    // 이 안에 있으면 공격
    public int damage = 1;
    public float attackCooldown = 1f;

    Transform player;
    float lastAttackTime = -999f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject p = GameObject.FindWithTag("Player");
        if (p != null) player = p.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null) return;

        Vector3 toPlayer = player.position - transform.position;
        toPlayer.y = 0;     // 높이는 무시
        float distance = toPlayer.magnitude;

        if (distance > detectRange) return;

        transform.forward = toPlayer.normalized;

        if (distance > attackRange)
        {
            float move = moveSpeed * Time.deltaTime;
            transform.position += transform.forward * move;
        }
        else if (Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;
            player.GetComponent<PayloadController>().TakeDamage(damage);
        }
    }
}
