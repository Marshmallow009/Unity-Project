using UnityEngine;

public class Sniper : MonoBehaviour, IDamageable
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
    public float attackRange = 15f;      // how far it can detect/shoot player
    public float shootCooldown = 2f;     // time between shots
    private float lastShotTime = 0f;

    [Header("Projectile Settings")]
    public GameObject projectilePrefab;  // assign enemy projectile prefab in Inspector
    public float projectileSpeed = 20f;

    [Header("Movement")]
    public float moveSpeed = 3f;
    private Transform player;
    private PlayerStats playerHealth; // cache player health script

    private Collider myCollider;
    public EnemyHealthBar healthBar;

    [Header("Other")]
    int rubbleLifetime = 5;
    public GameObject rubble;


    void Start()
    {
        currentHealth = maxHealth;
        myCollider = GetComponent<Collider>();

        // Find player and health script
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

        // Always look at player
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

        // Move closer if too far
        if (distance > attackRange)
        {
            transform.position += transform.forward * moveSpeed * Time.deltaTime;
        }
        else
        {
            // Shoot if within range and off cooldown
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
            Debug.LogWarning("⚠️ Missing projectilePrefab on Sniper.");
            return;
        }

        // Calculate direction to player
        Vector3 direction = (player.position - transform.position).normalized;

        // Spawn projectile slightly in front of Sniper to avoid instant collision
        Vector3 spawnPos = transform.position + direction * 1.5f;

        GameObject proj = Instantiate(projectilePrefab, spawnPos, Quaternion.LookRotation(direction));

        // ✅ Ignore collision between projectile and this enemy
        Collider projCol = proj.GetComponent<Collider>();
        if (projCol != null && myCollider != null)
        {
            Physics.IgnoreCollision(projCol, myCollider);
        }

        // ✅ Apply projectile speed
        Rigidbody rb = proj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = direction * projectileSpeed;
        }

        Debug.Log("🎯 Sniper fired a projectile at the player!");
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (healthBar != null)
            healthBar.UpdateHealth(currentHealth);
        Debug.Log($"Enemy took {amount} damage. Current health: {currentHealth}");

        if (currentHealth <= 0)
        {
            SniperDie();
        }
    }

    void SniperDie()
        {
            OnDeath?.Invoke();
            if (rubble != null)
            {
                for (int i = 0; i < 20; i++)
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
            Debug.Log("Enemy died! Dropping XP orbs...");
            int totalXP = Mathf.RoundToInt(enemyLevel * 1) + baseXP - 1; // or xp * enemyLevel if you have that variable

            if (xpPrefab != null)
            {
                Debug.Log($"Spawning {totalXP} XP orbs!");
                for (int i = 0; i < totalXP; i++)
                {
                    if (totalXP >= 10)
                    {
                        Debug.Log($"!!!Spawning {totalXP} XP orbs!!!");
                        Vector3 randomOffset = new Vector3(
                            Random.Range(-0.5f, 0.5f),
                            1f,
                            Random.Range(-0.5f, 0.5f)
                        );

                        GameObject tenxpOrb = Instantiate(xpPrefab[1], transform.position + randomOffset, Quaternion.identity);

                        OneXPOrb tenorb = tenxpOrb.GetComponent<OneXPOrb>();
                        if (tenorb != null)
                        {
                            tenorb.xpValue = 10;                    
                        }
                        totalXP = totalXP - 10;
                    }
                    if (totalXP >= 1 && totalXP < 10)
                    {
                        Debug.Log($"!!!Spawning {totalXP} XP orbs!!!");
                        Vector3 randomOffset = new Vector3(
                        Random.Range(-0.5f, 0.5f),
                        1f,
                        Random.Range(-0.5f, 0.5f)
                    );

                    GameObject xpOrb = Instantiate(xpPrefab[0], transform.position + randomOffset, Quaternion.identity);

                    OneXPOrb orb = xpOrb.GetComponent<OneXPOrb>();
                    if (orb != null)
                        orb.xpValue = 1;
                    }
                }
            }

            if (healthBar != null)
                Destroy(healthBar.gameObject);

            Destroy(gameObject);
        }
}
