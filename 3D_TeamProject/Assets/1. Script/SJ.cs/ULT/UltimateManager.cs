using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UltimateManager : MonoBehaviour
{
    [Header("±Ù°Å¸® ±Ã±Ø±â")]
    public MeleeUltimate meleeUltimate;

    [Header("¹«±â")]
    public WeaponSelect weaponSelect;

    [Header("±Ã±Ø±â UI")]
    public Image ultimateFillImage;
    public TextMeshProUGUI ultimatePercentText;

    [Header("±Ã±Ø±â ÃæÀü")]
    public int requiredKills = 10;
    public int currentKills = 0;

    private void Start()
    {
        UpdateUltimateUI();
    }

    private void Update()
    {
        if (Keyboard.current != null &&
            Keyboard.current.eKey.wasPressedThisFrame)
        {
            OnUltimate();
        }

        if (Keyboard.current != null &&
    Keyboard.current.f2Key.wasPressedThisFrame)
        {
            SoundManager.Instance.PlaySFX("SwordSkill");
            meleeUltimate.UseSkill();
        }
    }

    public void AddKill()
    {
        currentKills = Mathf.Min(currentKills + 1, requiredKills);

        UpdateUltimateUI();
    }

    public void ResetUltimate()
    {
        currentKills = 0;

        UpdateUltimateUI();
    }

    public void OnUltimate()
    {
        if (currentKills < requiredKills) return;
        if (weaponSelect == null) return;

        switch (weaponSelect.currentWeapon)
        {
            case WeaponType.Main:
                if (meleeUltimate == null || !meleeUltimate.UseSkill())
                    return;
                break;

            case WeaponType.Sub:
                SubSkill();
                break;

            default:
                return;
        }

        ResetUltimate();
    }

    private void MainSkill()
    {
        if (meleeUltimate == null) return;

        meleeUltimate.UseSkill();
    }

    private void SubSkill()
    {
        Debug.Log("¿ø°Å¸® ±Ã±Ø±â ½ºÅ³ »ç¿ë!");
    }

    private void UpdateUltimateUI()
    {
        float fillAmount =
            (float)currentKills / Mathf.Max(1, requiredKills);

        if (ultimateFillImage != null)
        {
            ultimateFillImage.fillAmount = fillAmount;
        }

        if (ultimatePercentText != null)
        {
            ultimatePercentText.text = $"{fillAmount * 100f:0}%";
        }
        
        if (currentKills >= requiredKills)
        {
            ultimatePercentText.text = $"{fillAmount * 100f:0}%" + " USE (E)";
        }
    }
}
