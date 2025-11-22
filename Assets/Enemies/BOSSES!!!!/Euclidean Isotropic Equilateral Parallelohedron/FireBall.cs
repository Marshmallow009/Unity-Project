using UnityEngine;

public class FireBall : MonoBehaviour
{
    public float speed = 40f;
    public float lifeTime = 5f;
    public int damage = 10;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody>();

        rb.useGravity = false;
        rb.isKinematic = false;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        // --- ROTATE TOWARD PLAYER ---
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Vector3 dir = (player.transform.position - transform.position).normalized;

            // Rotate to face the player
            transform.rotation = Quaternion.LookRotation(dir);
        }

        // --- MOVE FORWARD ---
        rb.linearVelocity = transform.forward * speed;

        Destroy(gameObject, lifeTime);
    }

    // --- COLLISION-BASED DETECTION ---
    private void OnCollisionEnter(Collision collision)
    {
        GameObject other = collision.gameObject;

        // Ignore boss & enemy
        if (other.CompareTag("Enemy") || other.CompareTag("Boss"))
            return;

        // Damage player
        if (other.CompareTag("Player"))
        {
            PlayerStats playerHealth = other.GetComponent<PlayerStats>();
            if (playerHealth != null)
                playerHealth.TakeDamage(damage);
        }

        // Destroy fireball
        Destroy(gameObject);
    }
}
