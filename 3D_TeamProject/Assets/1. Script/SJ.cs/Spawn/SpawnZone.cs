using UnityEngine;

public class SpawnZone : MonoBehaviour
{
    [Header("현재 구역 스포너")]
    public MonsterSpawner[] monsterSpawner;

    private bool isActivated = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (isActivated) return;

        isActivated = true;

        foreach (MonsterSpawner spawner in monsterSpawner)
        {
            if (spawner == null) continue;

            spawner.StartSpawn();
        }
    }
}