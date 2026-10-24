
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Health : MonoBehaviour
{
    [Header("체력")]
    [Tooltip("최대 체력")]
    public int maxHp = 3;

    [Tooltip("현재 오브젝트 체력")]
    public int currentHp;

    [Header("체력 UI")]
    public Slider hpslider;
    public TextMeshPro hpText;

    private UltimateManager ultimateManager;

    void Start()
    {
        currentHp = maxHp;
        ultimateManager = FindAnyObjectByType<UltimateManager>();
        UpdateBar();
    }

    public void TakeDamage(int damage)
    {
        if (currentHp <= 0) return;

        currentHp -= damage;
        currentHp = Mathf.Clamp(currentHp, 0, maxHp);

        Debug.Log(name + " Hp : " + currentHp);

        UpdateBar();

        if (currentHp <= 0)
            Die();
    }

    void UpdateBar()
    {
        if (hpslider != null)
        {
            hpslider.value = (float)currentHp / Mathf.Max(1, maxHp);
        }

        if (hpText != null)
        {
            hpText.text = $"{currentHp} / {maxHp}";
        }
    }

    void Die()
    {
        if (CompareTag("Player"))
        {
            Debug.Log("게임 오버");
            Time.timeScale = 0f;
        }
        else
        {
            if (ultimateManager != null)
            {
                ultimateManager.AddKill();
            }

            Destroy(gameObject);
        }
    }
}
