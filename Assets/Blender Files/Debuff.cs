using UnityEngine;

public class DebuffSpawner : MonoBehaviour
{
    public GameObject debuffPrefab;    // The debuff object to spawn
    public Vector3 spawnAreaMin;       // Minimum x,y,z of spawn area
    public Vector3 spawnAreaMax;       // Maximum x,y,z of spawn area
    public float spawnInterval = 5f;   // Time between spawns


    public Vector3 spinSpeed = new Vector3(0, 180, 0); // Degrees per second
    
    void Start()
    {
        InvokeRepeating(nameof(SpawnDebuff), 1f, spawnInterval);
    }

    void SpawnDebuff()
    {
        Vector3 randomPos = new Vector3(
            Random.Range(spawnAreaMin.x, spawnAreaMax.x),
            Random.Range(spawnAreaMin.y, spawnAreaMax.y),
            Random.Range(spawnAreaMin.z, spawnAreaMax.z)
        );

        Instantiate(debuffPrefab, randomPos, Quaternion.identity);
    }
    void Update()
    {
        transform.Rotate(spinSpeed * Time.deltaTime);
    }
}
