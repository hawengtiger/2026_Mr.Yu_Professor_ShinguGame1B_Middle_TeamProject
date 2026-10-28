
using UnityEngine;
using System.Collections;

public class MonsterSpawner : MonoBehaviour
{
    [Header("몬스터 종류")]
    public GameObject[] monsterPrefabs;

    [Header("스폰 설정")]
    public int spawnCount = 10;
    public float spawnInterval = 1f;
    public float spawnRadius = 5f;

    private Coroutine spawnCoroutine;

    public void StartSpawn()
    {
        if (spawnCoroutine != null) return;

        spawnCoroutine = StartCoroutine(SpawnRoutine());
    }

    public void StopSpawn()
    {
        if (spawnCoroutine == null) return;

        StopCoroutine(spawnCoroutine);
        spawnCoroutine = null;
    }

    private IEnumerator SpawnRoutine()
    {
        for (int i = 0 ; i < spawnCount ; i++)
        {
            SpawnMonster();

            yield return new WaitForSeconds(spawnInterval);
        }

        spawnCoroutine = null;
    }

    private void SpawnMonster()
    {
        if (monsterPrefabs == null || monsterPrefabs.Length == 0)
            return;

        // 몬스터 종류 랜덤 선택
        int randomMonster = Random.Range(0, monsterPrefabs.Length);

        GameObject monster = monsterPrefabs[randomMonster];

        if (monster == null) return;

        // 원형 범위 안의 랜덤 위치
        Vector2 randomPosition = Random.insideUnitCircle * spawnRadius;

        Vector3 spawnPosition = transform.position +
            new Vector3(randomPosition.x, 0f, randomPosition.y);

        Instantiate(monster, spawnPosition, monster.transform.rotation);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        const int segments = 64;

        Vector3 previousPoint = transform.position +
            Vector3.right * spawnRadius;

        for (int i = 1 ; i <= segments ; i++)
        {
            float angle = i * Mathf.PI * 2f / segments;

            Vector3 nextPoint = transform.position +
                new Vector3(
                    Mathf.Cos(angle) * spawnRadius,
                    0f,
                    Mathf.Sin(angle) * spawnRadius
                );

            Gizmos.DrawLine(previousPoint, nextPoint);

            previousPoint = nextPoint;
        }
    }
}
