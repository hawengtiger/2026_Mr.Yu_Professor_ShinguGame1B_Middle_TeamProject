using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using DG.Tweening;

public enum WeaponType
{
    Main,   // 근거리 무기
    Sub     // 원거리 무기
}

public class WeaponSelect : MonoBehaviour
{
    public GameObject sword;

    [Header("무기 이미지")]
    public Image mainWeaponImage;
    public Image subWeaponImage;

    [Header("궁극기 이미지")]
    public Image ultimateImage;
    public Image ultimateImage2;

    public Sprite mainUltimateSprite; // 근거리 궁극기
    public Sprite subUltimateSprite;  // 원거리 궁극기

    [Header("이동 시간")]
    public float moveDuration = 0.25f;

    [Header("투명도")]
    public float selectedAlpha = 1f;
    public float unselectedAlpha = 0.3f;

    private Vector2 mainPosition;
    private Vector2 subPosition;

    public WeaponType currentWeapon { get; private set; } = WeaponType.Main;

    private void Start()
    {
        mainPosition = mainWeaponImage.rectTransform.anchoredPosition;
        subPosition = subWeaponImage.rectTransform.anchoredPosition;

        mainWeaponImage.color = new Color(1f, 1f, 1f, selectedAlpha);
        subWeaponImage.color = new Color(1f, 1f, 1f, unselectedAlpha);

        // 처음에는 주무기 궁극기
        ultimateImage.sprite = mainUltimateSprite;
        ultimateImage2.sprite = mainUltimateSprite;
    }

    private void Update()
    {
        bool key1 = (Keyboard.current != null && Keyboard.current.digit1Key.wasPressedThisFrame)
            || (Mouse.current != null && Mouse.current.forwardButton.wasPressedThisFrame);

        bool key2 = (Keyboard.current != null && Keyboard.current.digit2Key.wasPressedThisFrame)
            || (Mouse.current != null && Mouse.current.backButton.wasPressedThisFrame);

        // 1번과 2번 동시 입력 금지
        if (key1 && key2)
        {
            return;
        }

        if (key1)
        {
            sword.SetActive(true);
            SelectWeapon(WeaponType.Main);
        }

        if (key2)
        {
            sword.SetActive(false);
            SelectWeapon(WeaponType.Sub);
        }
    }

    public void SelectWeapon(WeaponType weapon)
    {
        if (currentWeapon == weapon)
            return;

        currentWeapon = weapon;

        mainWeaponImage.rectTransform.DOKill();
        subWeaponImage.rectTransform.DOKill();
        mainWeaponImage.DOKill();
        subWeaponImage.DOKill();

        switch (currentWeapon)
        {
            case WeaponType.Main:

                mainWeaponImage.rectTransform.DOAnchorPos(mainPosition, moveDuration);
                subWeaponImage.rectTransform.DOAnchorPos(subPosition, moveDuration);

                mainWeaponImage.DOFade(selectedAlpha, moveDuration);
                subWeaponImage.DOFade(unselectedAlpha, moveDuration);

                mainWeaponImage.transform.SetAsLastSibling();

                // 근거리 궁극기로 변경
                ultimateImage.sprite = mainUltimateSprite;
                ultimateImage2.sprite = mainUltimateSprite;

                break;

            case WeaponType.Sub:

                mainWeaponImage.rectTransform.DOAnchorPos(subPosition, moveDuration);
                subWeaponImage.rectTransform.DOAnchorPos(mainPosition, moveDuration);

                mainWeaponImage.DOFade(unselectedAlpha, moveDuration);
                subWeaponImage.DOFade(selectedAlpha, moveDuration);

                subWeaponImage.transform.SetAsLastSibling();

                // 원거리 궁극기로 변경
                ultimateImage.sprite = subUltimateSprite;
                ultimateImage2.sprite = subUltimateSprite;

                break;
        }
    }
}
