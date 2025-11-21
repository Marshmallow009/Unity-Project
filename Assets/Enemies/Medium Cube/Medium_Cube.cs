using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MediumCube : MonoBehaviour, IDamageable
{
    public event System.Action OnDeath;

    [Header("Enemy Stats")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("XP Drop")]
    public GameObject[] xpPrefab;
    public int baseXP = 5;
    public int enemyLevel = 1;

    [Header("Combat")]
    public int attackDamage = 10;
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;
    private float lastAttackTime = 0f;

    [Header("Movement")]
    public float moveSpeed = 3f;
    private Transform player;
    private PlayerStats playerHealth;

    [Header("HealthBar")]
    public GameObject healthBarPrefab; // assign your prefab in Inspector
    private EnemyHealthBar healthBarInstance;

    [Header("Other")]
    int rubbleLifetime = 5;
    public GameObject rubble;


    
    void Start()
    {
        currentHealth = maxHealth;
        // Find the player
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            playerHealth = playerObj.GetComponent<PlayerStats>();
            if (playerHealth != null)
            {
                enemyLevel = playerHealth.playerLevel + 2;
                Debug.Log($"enemyLevel:{enemyLevel}");
            }
        }
        if (healthBarPrefab != null)
        {
            GameObject hbObj = Instantiate(healthBarPrefab); // instantiate prefab
            healthBarInstance = hbObj.GetComponent<EnemyHealthBar>();
            if (healthBarInstance != null)
                healthBarInstance.Initialize(transform, maxHealth, 2f); // 2f is height above enemy
        }
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // Look at player
        Vector3 dir = (player.position - transform.position).normalized;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 5f);

        // Move toward player if far
        if (distance > attackRange)
            transform.position += transform.forward * moveSpeed * Time.deltaTime;
        else
            AttackIfReady();
    }
    void AttackIfReady()
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            playerHealth?.TakeDamage(attackDamage);
            lastAttackTime = Time.time;
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth < 0) currentHealth = 0;

        if (currentHealth <= 0)
            Die();
    }

    void Die()
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

        // Destroy health bar
        if (healthBarInstance != null)
            Destroy(healthBarInstance.gameObject);

        Destroy(gameObject);
    }

}