using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage = 1;
    public float lifeTime = 2f;     // 2초 뒤 삭제
    public string targetTag = "Enemy";      // 맞히면 데미지
    public string ownerTag = "Player";        // 쏜 쪽은 통과

    PayloadController payload;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        payload = FindAnyObjectByType<PayloadController>();
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;
        if (other.CompareTag(ownerTag)) return;

        if (other.CompareTag(targetTag))
        {
            Health hp = other.GetComponent<Health>();
            if (hp != null) hp.TakeDamage(damage);
        }

        if (other.CompareTag(targetTag = "Player"))
        {
            payload.TakeDamage(damage);
        }
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
