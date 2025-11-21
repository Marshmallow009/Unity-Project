using UnityEngine;

public class Cannonball : MonoBehaviour
{
    [Header("Projectile Settings")]
    public int damage = 10;
    public float speed = 20f;
    public float lifetime = 5f;

    [Header("Effects")]
    public GameObject explosionEffect;

    [Header("Trail Settings")]
    public GameObject trailPrefab; // Assign prefab with TrailRenderer
    private GameObject activeTrail;

    private Vector3 moveDirection;

    void Start()
    {
        // Auto-destroy after lifetime
        Destroy(gameObject, lifetime);

        // Spawn trail and parent to projectile
        if (trailPrefab != null)
        {
            activeTrail = Instantiate(trailPrefab, transform.position, Quaternion.identity, transform);
        }

        // Make collider a trigger
        Collider myCollider = GetComponent<Collider>();
        if (myCollider != null)
            myCollider.isTrigger = true;
    }

    void Update()
    {
        // Move projectile forward
        transform.position += moveDirection * speed * Time.deltaTime;
    }

    /// <summary>
    /// Call this to launch the projectile toward a position
    /// </summary>
    public void Launch(Vector3 targetPosition)
    {
        moveDirection = (targetPosition - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(moveDirection);
    }

    void OnTriggerEnter(Collider other)
    {
        // Only damage player
        if (other.CompareTag("Player"))
        {
            PlayerStats player = other.GetComponent<PlayerStats>();
            if (player != null)
            {
                player.TakeDamage(damage);
                Debug.Log($"Enemy projectile hit {other.name} for {damage} damage!");
            }
        }

        // Spawn explosion effect
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        // Destroy projectile
        Destroy(gameObject);
    }

    void OnDestroy()
    {
        // Detach trail and let it fade naturally
        if (activeTrail != null)
        {
            activeTrail.transform.parent = null;
            TrailRenderer tr = activeTrail.GetComponent<TrailRenderer>();
            if (tr != null)
            {
                tr.emitting = false;
                tr.autodestruct = true; // ensures it deletes itself
            }
        }
    }
}
