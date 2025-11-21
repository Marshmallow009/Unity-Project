using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class SmallBoi : MonoBehaviour, IDamageable
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
    public float attackRange = 4f;
    public float attackCooldown = 1.5f;
    private float lastAttackTime = 0f;

    [Header("Movement")]
    public float moveSpeed = 3f;
    private Transform player;
    private PlayerStats playerHealth;
    public float hopHeight = 6f;
    public float hopDuration = 1;
    public float hopDistance = 3f;
    private bool isHopping = false;

    [Header("HealthBar")]
    public GameObject healthBarPrefab; // assign your prefab in Inspector
    private EnemyHealthBar healthBarInstance;

    [Header("Other")]
    int rubbleLifetime = 5;
    public GameObject rubble;




    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        if (player != null)
        {
            playerHealth = player.GetComponent<PlayerStats>(); // 👈 Assign this
            Vector3 pos = transform.position;
            pos.y = 1;
            transform.position = pos;
        }
    }


    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        Vector3 dir = (player.position - transform.position).normalized;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 5f);

        // Only hop if not already hopping and player is not too close
        if (!isHopping && distance > attackRange)
        {
            StartCoroutine(HopTowardPlayer());
        }
        else if (distance <= attackRange)
        {
            AttackIfReady();
        }
    }

    IEnumerator HopTowardPlayer()
    {
    isHopping = true;

    Vector3 startPos = transform.position;

    // Direction toward player (flattened on the ground)
    Vector3 dir = (player.position - startPos).normalized;
    dir.y = 0f;

    Vector3 endPos = startPos + dir * hopDistance;

    // Prevent overshooting (if player is closer than hopDistance)
    float distanceToPlayer = Vector3.Distance(startPos, player.position);
    if (distanceToPlayer < hopDistance)
        endPos = player.position; // stop just in front of them

    float elapsed = 0f;

    yield return new WaitForSeconds(0.1f); // short delay before hop

    while (elapsed < hopDuration)
    {
        elapsed += Time.deltaTime;
        float t = elapsed / hopDuration;

        // Smooth horizontal move
        transform.position = Vector3.Lerp(startPos, endPos, t);

        // Vertical hop using sine curve
        float height = Mathf.Sin(t * Mathf.PI) * hopHeight;
        transform.position = new Vector3(transform.position.x, startPos.y + height, transform.position.z);

        yield return null;
    }

    // Land back down
    transform.position = new Vector3(endPos.x, startPos.y, endPos.z);

    // Play squish animation on landing
    anim.SetTrigger("Squish");

    yield return new WaitForSeconds(0.42f); // wait for squish animation to finish
    isHopping = false;
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
            smallBoi();
    }

    void smallBoi()
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
