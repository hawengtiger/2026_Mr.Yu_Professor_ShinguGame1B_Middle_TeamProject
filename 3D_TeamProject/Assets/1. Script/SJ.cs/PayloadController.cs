using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class PayloadController : MonoBehaviour
{
    [Header("화물 이동")]
    public float moveSpeed = 1.75f;
    public float moveDistance = 525f;

    [Header("화물 체력")]
    public int maxHp = 3500;
    public int currentHp;

    [Header("화물 체력 UI")]
    public Image hpImage;
    public TextMeshProUGUI hpText;

    [Header("화물 진행도 UI")]
    public Slider moveSlider;

    private Vector3 startPosition;

    private bool isDestroyed = false;
    private bool isArrived = false;

    private void Start()
    {
        // 시작 위치 저장
        startPosition = transform.position;

        // 체력 초기화
        currentHp = maxHp;

        // 체력 UI 초기화
        UpdateHpUI();

        // Slider 초기화
        moveSlider.minValue = 0f;
        moveSlider.maxValue = 1f;
        moveSlider.value = 0f;
    }

    private void Update()
    {
        Move();
        UpdateMoveSlider();

        // 테스트용 : F1을 누르면 화물 체력 100 감소
        if (Keyboard.current.f1Key.isPressed)
        {
            TakeDamage(1);
        } 
    }

    private void Move()
    {
        // 파괴되었거나 목적지에 도착했다면 이동 X
        if (isDestroyed || isArrived)
            return;

        // 현재까지 이동한 거리
        float currentDistance =
            Vector3.Distance(startPosition, transform.position);

        // 목적지 도착
        if (currentDistance >= moveDistance)
        {
            isArrived = true;

            Debug.Log("화물이 목적지에 도착했습니다.");
            return;
        }

        // 화물 자동 이동
        transform.position += transform.forward * moveSpeed * Time.deltaTime;
    }

    private void UpdateMoveSlider()
    {
        // 현재까지 이동한 거리
        float currentDistance =
            Vector3.Distance(startPosition, transform.position);

        // 0 ~ 1 사이의 진행도로 변환
        float progress = currentDistance / moveDistance;

        // Slider에 적용
        moveSlider.value = Mathf.Clamp01(progress);
    }

    public void TakeDamage(int damage)
    {
        if (isDestroyed)
            return;

        currentHp -= damage;

        currentHp = Mathf.Clamp(currentHp, 0, maxHp);

        // 체력 UI 갱신
        UpdateHpUI();

        if (currentHp <= 0)
        {
            DestroyPayload();
        }
    }

    private void UpdateHpUI()
    {
        // 체력바
        hpImage.fillAmount = (float)currentHp / maxHp;

        // 체력 텍스트
        hpText.text = "HP : " + currentHp + " / " + maxHp;
    }

    private void DestroyPayload()
    {
        isDestroyed = true;

        Debug.Log("화물이 파괴되었습니다.");
    }
}