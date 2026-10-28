
using UnityEngine;
using System.Collections.Generic;

public class PayloadAuraHitBox : MonoBehaviour
{
    [Header("애니메이션")]
    public Animator animator;

    [SerializeField] private List<GameObject> enemies = new List<GameObject>();

    private PayloadController payload;
    private float defaultSpeed;

    private void Start()
    {
        payload = GetComponentInParent<PayloadController>();

        if (payload != null)
        {
            // 원래 속도를 안전하게 저장함
            defaultSpeed = payload.moveSpeed;
        }
        else
        {
            Debug.LogError("부모 오브젝트에서 PayloadController를 찾을 수 없습니다!");
        }
    }

    private void Update()
    {
        if (payload == null)
            return;

        // 죽거나 비활성화된 몬스터 제거
        enemies.RemoveAll(enemy => enemy == null || !enemy.activeInHierarchy);

        if (enemies.Count > 0)
        {
            // 적이 있으면 화물 정지
            payload.moveSpeed = 0f;

            if (animator != null)
                animator.SetFloat("Stop", 1f);
        }
        else
        {
            // 적이 없으면 화물 이동
            payload.moveSpeed = defaultSpeed;

            if (animator != null)
                animator.SetFloat("Stop", 0f);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            if (!enemies.Contains(other.gameObject))
            {
                enemies.Add(other.gameObject);
            }

            Debug.Log("적이 영역에 들어옴 -> 화물 정지");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            enemies.Remove(other.gameObject);

            Debug.Log("적이 영역에서 나감 -> 화물 이동 재개");
        }
    }
}
