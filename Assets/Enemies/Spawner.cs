using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject[] enemyPrefabs; // multiple enemy types
    public float spawnInterval = 5f;
    public int maxEnemies = 10;
    public Transform[] spawnPoints;

    private int currentEnemyCount = 0;

    void Start()
    {
        StartCoroutine(SpawnTimer());
    }

    IEnumerator SpawnTimer()
    {
        while (true)
        {
            if (currentEnemyCount < maxEnemies)
            {
                SpawnEnemy();
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    void SpawnEnemy()
    {
        // Pick a random enemy type
        GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

        // Pick a random spawn point
        Transform spawnPoint = spawnPoints.Length > 0 ?
            spawnPoints[Random.Range(0, spawnPoints.Length)] :
            transform;

        GameObject newEnemy = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
        currentEnemyCount++;

        // Try to attach to IDamageable event
        IDamageable damageable = newEnemy.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.OnDeath += EnemyDied;
        }

        Debug.Log($"Spawned new enemy ({prefab.name}). Total enemies: {currentEnemyCount}");
    }

    void EnemyDied()
    {
        currentEnemyCount--;
    }
}
