using UnityEngine;

public class Cannon : MonoBehaviour, IDamageable
{
    public event System.Action OnDeath;

    [Header("Enemy Stats")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("XP Drop")]
    public GameObject[] xpPrefab;
    public int baseXP = 7;
    public int enemyLevel = 1;     

    [Header("Combat")]
    public int attackDamage = 10;
    public float attackRange = 15f;
    public float shootCooldown = 2f;
    private float lastShotTime = 0f;

    [Header("Projectile Settings")]
    public GameObject projectilePrefab;
    public float projectileSpeed = 20f;

    [Header("Movement")]
    public float moveSpeed = 3f;
    private Transform player;
    private PlayerStats playerHealth;

    private Collider myCollider;
    public EnemyHealthBar healthBar;

    // ✅ NEW
    private Transform shootPoint;

    [Header("Other")]
    int rubbleLifetime = 5;
    public GameObject rubble;

    void Start()
    {
        currentHealth = maxHealth;
        myCollider = GetComponent<Collider>();

        // ✅ Locate ShootPoint in children
        shootPoint = transform.Find("ShootPoint");

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
            playerHealth = playerObject.GetComponent<PlayerStats>();
        }
        else
        {
            Debug.LogError("❌ Player with tag 'Player' not found! Please tag your player object correctly.");
        }
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

        if (distance > attackRange)
        {
            transform.position += transform.forward * moveSpeed * Time.deltaTime;
        }
        else
        {
            if (Time.time >= lastShotTime + shootCooldown)
            {
                ShootAtPlayer();
                lastShotTime = Time.time;
            }
        }
    }

    void ShootAtPlayer()
    {
        if (projectilePrefab == null)
        {
            Debug.LogWarning("⚠️ Missing projectilePrefab on Cannon.");
            return;
        }

        if (shootPoint == null)
        {
            Debug.LogError("❌ Cannot shoot: ShootPoint reference missing.");
            return;
        }

        // ✅ Spawn at shootPoint position & direction
        GameObject proj = Instantiate(projectilePrefab, shootPoint.position, shootPoint.rotation);

        // Ignore collision with itself
        Collider projCol = proj.GetComponent<Collider>();
        if (projCol != null && myCollider != null)
        {
            Physics.IgnoreCollision(projCol, myCollider);
        }

        Rigidbody rb = proj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = shootPoint.forward * projectileSpeed;
        }

        Debug.Log("💥 Cannon fired!");
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (healthBar != null)
            healthBar.UpdateHealth(currentHealth);

        if (currentHealth <= 0)
        {
            cannonDie();
        }
    }

    void cannonDie()
    {
        OnDeath?.Invoke();
                if (rubble != null)
        {
            for (int i = 0; i < 50; i++)
            {
                Vector3 randomOffset = new Vector3(
                    Random.Range(-1f, 1f),
                    1f,
                    Random.Range(-1f, 1f)
                );

                GameObject piece = Instantiate(
                    rubble,
                    transform.position + randomOffset,
                    Quaternion.identity
                );

                Destroy(piece, rubbleLifetime); // <-- correct instance destruction
            }
        }
        int totalXP = Mathf.RoundToInt(enemyLevel * 1) + baseXP;

        if (xpPrefab != null && xpPrefab.Length > 0)
        {
            // Spawn "10 XP orbs" first
            while (totalXP >= 10)
            {
                Vector3 randomOffset = new Vector3(
                    Random.Range(-0.5f, 0.5f),
                    1f,
                    Random.Range(-0.5f, 0.5f)
                );
                GameObject tenOrb = Instantiate(xpPrefab.Length > 1 ? xpPrefab[1] : xpPrefab[0], transform.position + randomOffset, Quaternion.identity);
                OneXPOrb orbScript = tenOrb.GetComponent<OneXPOrb>();
                if (orbScript != null) orbScript.xpValue = 10;

                totalXP -= 10;
            }

            // Spawn remaining "1 XP orbs"
            for (int i = 0; i < totalXP; i++)
            {
                Vector3 randomOffset = new Vector3(
                    Random.Range(-0.5f, 0.5f),
                    1f,
                    Random.Range(-0.5f, 0.5f)
                );
                GameObject orb = Instantiate(xpPrefab[0], transform.position + randomOffset, Quaternion.identity);
                OneXPOrb orbScript = orb.GetComponent<OneXPOrb>();
                if (orbScript != null) orbScript.xpValue = 1;
            }
        }
        Destroy(gameObject);
    }
}
