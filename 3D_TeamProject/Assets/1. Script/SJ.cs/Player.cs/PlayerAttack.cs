
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using System.Collections.Generic;
using Unity.VisualScripting;

public class PlayerAttack : MonoBehaviour
{
    [Header("칼날 잔상")]
    public TrailRenderer swordTrail;
    public Transform swordPivot;

    public float swingAngle = 120f;
    public float swingDuration = 0.2f;

    [Header("근거리 공격")]
    public PayloadController payload;
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public int damage = 1;
    public LayerMask enemyLayer;

    private HashSet<Health> hitEnemies = new HashSet<Health>();

    public WeaponSelect weaponSelect;
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 20f;
    public float fireCooldown = 0.2f;

    float lastFireTime = -999f;
    private bool isAttacking;

    public void OnAttack()
    {
        switch (weaponSelect.currentWeapon)
        {
            case WeaponType.Main:
                ShoartAttack();
                break;

            case WeaponType.Sub:
                if (Time.timeScale == 0f) return;
                if (Time.time < lastFireTime + fireCooldown) return;
                lastFireTime = Time.time;


                GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
                Rigidbody rb = bullet.GetComponent<Rigidbody>();
                rb.linearVelocity = firePoint.right * bulletSpeed + firePoint.up * 2;
                break;
        }
    }

    public void ShoartAttack()
    {
        if (isAttacking) return;

        isAttacking = true;
        hitEnemies.Clear();

        swordPivot.DOKill();

        swordTrail.emitting = false;
        swordTrail.Clear();

        swordPivot.localRotation =
            Quaternion.Euler(0f, 0f, 0f);

        swordTrail.emitting = true;

        swordPivot
            .DOLocalRotate(
                new Vector3(0f, -180f, 0f),
                swingDuration,
                RotateMode.FastBeyond360
            )
            .SetEase(Ease.OutQuad)
            .OnUpdate(() =>
            {
                CheckSwordHit();
            })
            .OnComplete(() =>
            {
                swordTrail.emitting = false;

                swordPivot.localRotation =
                    Quaternion.Euler(0f, 0f, 0f);

                isAttacking = false;
            });
    }

    private void CheckSwordHit()
    {
        if (!isAttacking || attackPoint == null) return;

        Collider[] hits = Physics.OverlapSphere(
            attackPoint.position,
            attackRange,
            enemyLayer
        );

        foreach (Collider hit in hits)
        {
            Health health = hit.GetComponentInParent<Health>();

            if (health == null) continue;
            if (hitEnemies.Contains(health)) continue;

            health.TakeDamage(damage);
            hitEnemies.Add(health);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
