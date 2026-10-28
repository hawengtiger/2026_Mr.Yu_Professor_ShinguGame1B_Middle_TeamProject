
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MeleeUltimate : MonoBehaviour
{
    [Header("궁극기 VFX")]
    public GameObject ultimateVFX;
    public Transform vfxSpawnPoint;
    public float vfxLifeTime = 2f;

    [Header("검")]
    public Transform swordPivot;
    public TrailRenderer swordTrail;

    [Header("공격 설정")]
    public Transform attackPoint;
    public float attackRange = 3f;
    public int damage = 100;
    public LayerMask enemyLayer;

    [Header("경고 원")]
    public GameObject warningCircle;
    public float warningDuration = 0.8f;
    public float blinkInterval = 0.1f;

    [Header("내려찍기")]
    public float slamAngle = -180f;
    public float slamDuration = 0.25f;
    public float returnDuration = 0.3f;

    private bool isUsing = false;
    public bool IsUsing => isUsing;

    private Coroutine skillCoroutine;
    private Tween swordTween;

    private void Start()
    {
        warningCircle.SetActive(false);
    }


    public bool UseSkill()
    {
        if (isUsing || swordPivot == null || attackPoint == null)
            return false;

        isUsing = true;
        skillCoroutine = StartCoroutine(SkillRoutine());

        return true;
    }

    private IEnumerator SkillRoutine()
    {
        // 1. 경고 원 점멸
        if (warningCircle != null)
        {
            warningCircle.SetActive(false);

            float elapsed = 0f;
            bool visible = false;

            while (elapsed < warningDuration)
            {
                visible = !visible;
                warningCircle.SetActive(visible);

                float wait = Mathf.Min(
                    Mathf.Max(0.01f, blinkInterval),
                    warningDuration - elapsed
                );

                yield return new WaitForSeconds(wait);
                elapsed += wait;
            }

            warningCircle.SetActive(false);
        }
        else
        {
            yield return new WaitForSeconds(warningDuration);
        }

        // 2. 검 내려찍기
        swordPivot.DOKill();

        Vector3 originalRotation = swordPivot.localEulerAngles;

        if (swordTrail != null)
        {
            swordTrail.Clear();
            swordTrail.emitting = true;
        }

        bool slamFinished = false;

        swordTween = swordPivot.DOLocalRotate(
            originalRotation + new Vector3(0f, slamAngle, 0f),
            slamDuration,
            RotateMode.FastBeyond360
        )
        .SetEase(Ease.InExpo)
        .OnComplete(() => slamFinished = true);

        yield return new WaitUntil(() => slamFinished);

        // 3. 강타 순간 범위 피해
        PlayUltimateVFX();
        ApplyDamage();

        // 4. 검 원래 각도로 복귀
        bool returnFinished = false;

        swordTween = swordPivot.DOLocalRotate(
            originalRotation,
            returnDuration,
            RotateMode.FastBeyond360
        )
        .SetEase(Ease.OutQuad)
        .OnComplete(() => returnFinished = true);

        yield return new WaitUntil(() => returnFinished);

        if (swordTrail != null)
            swordTrail.emitting = false;

        isUsing = false;
        skillCoroutine = null;
    }

    private void ApplyDamage()
    {
        Collider[] enemies = Physics.OverlapSphere(
            attackPoint.position,
            attackRange,
            enemyLayer
        );

        HashSet<Health> damagedEnemies = new HashSet<Health>();

        foreach (Collider enemy in enemies)
        {
            Health health = enemy.GetComponentInParent<Health>();

            if (health == null) continue;

            if (!damagedEnemies.Add(health)) continue;

            health.TakeDamage(damage);
        }

        Debug.Log($"지면 강타! {damagedEnemies.Count}마리 공격");
    }

    private void OnDisable()
    {
        if (skillCoroutine != null)
        {
            StopCoroutine(skillCoroutine);
            skillCoroutine = null;
        }

        if (swordTween != null && swordTween.IsActive())
            swordTween.Kill();

        if (warningCircle != null)
            warningCircle.SetActive(false);

        if (swordTrail != null)
            swordTrail.emitting = false;

        isUsing = false;
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(
            attackPoint.position,
            attackRange
        );
    }

    private void PlayUltimateVFX()
    {
        if (ultimateVFX == null) return;

        Vector3 spawnPos = vfxSpawnPoint != null
            ? vfxSpawnPoint.position
            : transform.position;

        GameObject effect = Instantiate(
            ultimateVFX,
            spawnPos,
            Quaternion.identity
        );

        Destroy(effect, vfxLifeTime);
    }
}
