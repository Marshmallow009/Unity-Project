using UnityEngine;

public class HomingFireball : MonoBehaviour
{
    [Header("Projectile Settings")]
    public int damage = 10;
    public float speed = 15f;
    public float rotationSpeed = 2f;
    public float lifetime = 5f;

    [Tooltip("Explodes if it gets too close (prevents hovering)")]
    public float minimumDistanceToExplode = 1.2f;

    [Tooltip("Explodes if velocity magnitude drops below this")]
    public float minVelocityBeforeExplode = 1.0f;

    [Header("Explosion Settings")]
    public float explosionRadius = 3f;
    public GameObject explosionEffect;

    private Transform player;
    private Rigidbody rb;
    private float elapsedTime = 0f;
    private float homingDuration;
    private bool isHoming = true;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody>();

        rb.useGravity = false;
        rb.isKinematic = false;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        // Make sure projectile collider is trigger (important!)
        Collider col = GetComponent<Collider>();
        if (col != null) col.isTrigger = true;

        // Ignore enemy collisions (requires enemies on "Enemy" layer)
        int enemyLayer = LayerMask.NameToLayer("Enemy");
        if (enemyLayer != -1)
        {
            Physics.IgnoreLayerCollision(gameObject.layer, enemyLayer, true);
        }

        // Find player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        // Homing only during first quarter of lifetime
        homingDuration = lifetime * 0.15f;
    }

    void FixedUpdate()
    {
        elapsedTime += Time.fixedDeltaTime;

        if (player == null)
        {
            Explode();
            return;
        }

        // --------------------------
        // HOMING LOGIC
        // --------------------------
        if (isHoming && elapsedTime <= homingDuration)
        {
            Vector3 direction = (player.position - transform.position).normalized;

            // Prevent extreme vertical flipping
            direction = Vector3.ProjectOnPlane(direction, Vector3.up).normalized +
                        Vector3.up * Mathf.Clamp(direction.y, -0.5f, 0.5f);

            Quaternion targetRot = Quaternion.LookRotation(direction);
            rb.rotation = Quaternion.RotateTowards(
                rb.rotation,
                targetRot,
                rotationSpeed * 50f * Time.fixedDeltaTime
            );
        }
        else
        {
            isHoming = false;
        }

        // Always move forward
        rb.linearVelocity = transform.forward * speed;

        float dist = Vector3.Distance(transform.position, player.position);

        // Early detonation if too close
        if (dist <= minimumDistanceToExplode)
            Explode();

        // Detonate if stuck or slowed down
        if (rb.linearVelocity.magnitude < minVelocityBeforeExplode)
            Explode();

        if (elapsedTime >= lifetime)
            Explode();
    }

    // Trigger collision with walls / objects
    void OnTriggerEnter(Collider other)
    {
        // Ignore enemies
        if (other.CompareTag("Enemy"))
            return;

        // Hit something → explode
        Explode();
    }

    void Explode()
    {
        if (explosionEffect != null)
        {
            GameObject expl = Instantiate(explosionEffect, transform.position, Quaternion.identity);
            Destroy(expl, 2f); // Cleanup explosion effect
        }

        // Damage anything with PlayerStats within radius
        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius, ~0);

        foreach (Collider hit in hits)
        {
            PlayerStats stats = hit.GetComponentInParent<PlayerStats>();
            if (stats != null)
            {
                stats.TakeDamage(damage);
                Debug.Log($"🔥 Explosion hit player for {damage} damage!");
            }
        }

        Destroy(gameObject); // Destroy the fireball itself
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
