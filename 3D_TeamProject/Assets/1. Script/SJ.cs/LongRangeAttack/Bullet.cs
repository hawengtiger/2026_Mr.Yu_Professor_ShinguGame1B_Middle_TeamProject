
using UnityEngine;

public enum BulletType
{
    Player,
    Enemy
}

public class Bullet : MonoBehaviour
{


    [Header("총알 설정")]
    public BulletType bulletType;
    public int damage = 1;
    public float lifeTime = 2f;
    public string targetTag = "Enemy";
    public string ownerTag = "Player";

    [Header("총알 회전")]
    public Transform bulletModel;
    public Vector3 modelRotationOffset = Vector3.zero;
    public float rotationSpeed = 720f;

    private Rigidbody rb;
    private PayloadController payload;
    private float rotationAngle;

    void Start()
    {
        payload = FindAnyObjectByType<PayloadController>();
        rb = GetComponent<Rigidbody>();

        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        if (bulletModel == null)
            return;

        // 총알 진행 방향으로 부모 회전
        if (rb != null && rb.linearVelocity.sqrMagnitude > 0.01f)
        {
            transform.rotation =
                Quaternion.LookRotation(rb.linearVelocity.normalized);
        }

        rotationAngle += rotationSpeed * Time.deltaTime;

        // 총알 종류에 따라 회전축 변경
        Vector3 rotationAxis =
            bulletType == BulletType.Player
                ? Vector3.forward  // 플레이어: Z축
                : Vector3.up;      // 적: Y축

        bulletModel.localRotation =
            Quaternion.Euler(modelRotationOffset) *
            Quaternion.AngleAxis(rotationAngle, rotationAxis);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;
        if (other.CompareTag(ownerTag)) return;

        if (other.CompareTag(targetTag))
        {
            Health hp = other.GetComponentInParent<Health>();

            if (hp != null)
                hp.TakeDamage(damage);
        }

        if (other.CompareTag("Player"))
        {
            if (payload != null)
                payload.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}
