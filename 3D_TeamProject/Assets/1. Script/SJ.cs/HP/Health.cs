using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [Header("플레이어만 연결")]
    [Tooltip("최대 체력")]
    public int maxHp = 3;
    [Tooltip("현재 오브젝트 체력")]
    public int currentHp;

    [Header("플레이어만 연결")]
    [Tooltip("적 오브젝트에는 이걸 붙이지 않아도 됩니다.")]
    public Slider hpslider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHp = maxHp;
        UpdateBar();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int damage)
    {
        if (currentHp <= 0) return;     // 죽었으면 무시

        currentHp -= damage;
        Debug.Log(name + "Hp : " + currentHp);
        UpdateBar();

        if (currentHp <= 0) Die();
    }

    void UpdateBar()
    {
        if (hpslider != null)
            hpslider.value = (float)currentHp / maxHp;
    }

    void Die()
    {
        if (CompareTag("Player"))
        {
            Debug.Log("게임 오버");
            Time.timeScale = 0f;        // 게임 정지
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
