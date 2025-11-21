using UnityEngine;

public class FireShot : MonoBehaviour
{
    public float speed = 40f;
    public float lifeTime = 5f;
    public int damage = 10;
    public TrailRenderer trail;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody>();

        rb.isKinematic = false;
        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        // Launch forward
        rb.linearVelocity = transform.forward * speed;

        if (trail != null)
            trail.Clear();

        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStats playerHealth = other.GetComponent<PlayerStats>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                Debug.Log($"💥 FireShot hit player for {damage} damage!");
            }

            Destroy(gameObject);
        }
    }
}
