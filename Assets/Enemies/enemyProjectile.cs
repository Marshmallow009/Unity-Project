using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    public int damage = 10;
    public float speed = 15f;
    public float lifetime = 5f;

    void Start()
    {
        Destroy(gameObject, lifetime);

        // Ignore collisions with all objects tagged "Projectile"
        foreach (var other in GameObject.FindGameObjectsWithTag("Projectile"))
        {
            if (other != gameObject)
            {
                Collider c1 = GetComponent<Collider>();
                Collider c2 = other.GetComponent<Collider>();
                if (c1 != null && c2 != null)
                    Physics.IgnoreCollision(c1, c2);
            }
        }

        // Ignore collisions with all objects tagged "Enemy"
        foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            Collider c1 = GetComponent<Collider>();
            Collider c2 = enemy.GetComponent<Collider>();
            if (c1 != null && c2 != null)
                Physics.IgnoreCollision(c1, c2);
        }
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        // Only damage the player
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerStats playerHealth = collision.gameObject.GetComponent<PlayerStats>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                Debug.Log($"💥 Enemy projectile hit player for {damage} damage!");
            }
        }

        Destroy(gameObject);
    }
}
