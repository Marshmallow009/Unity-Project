using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyFireController : MonoBehaviour
{
    [Header("Projectile Settings")]
    public GameObject projectilePrefab;
    public GameObject trailPrefab;          // Assign trail prefab here
    public float detectionRange = 50f;
    public float fireCooldown = 1f;

    private List<Transform> firePoints = new List<Transform>();
    private Transform player;
    private bool canShoot = true;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        foreach (Transform child in transform)
        {
            if (child.name.Contains("FirePoint"))
            {
                firePoints.Add(child);
            }
        }
    }

    void Update()
    {
        if (player == null) return;

        foreach (Transform point in firePoints)
        {
            TryShootFromPoint(point);
        }
    }

    void TryShootFromPoint(Transform firePoint)
    {
        if (!canShoot) return;

        Vector3 dir = (player.position - firePoint.position).normalized;

        if (Physics.Raycast(firePoint.position, dir, out RaycastHit hit, detectionRange))
        {
            if (hit.collider.CompareTag("Player"))
            {
                StartCoroutine(FireFromPoint(firePoint, dir));
            }
        }
    }

    IEnumerator FireFromPoint(Transform point, Vector3 dir)
    {
        canShoot = false;

        // Spawn projectile
        GameObject proj = Instantiate(projectilePrefab, point.position, Quaternion.LookRotation(dir));

        // Attach trail if available
        if (trailPrefab != null)
        {
            GameObject trailInstance = Instantiate(trailPrefab, proj.transform.position, Quaternion.identity, proj.transform);
            // Optional: configure TrailRenderer settings here
        }

        yield return new WaitForSeconds(fireCooldown);
        canShoot = true;
    }
}
