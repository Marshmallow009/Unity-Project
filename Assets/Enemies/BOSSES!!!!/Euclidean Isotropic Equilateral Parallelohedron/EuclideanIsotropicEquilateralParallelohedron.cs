using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class EuclideanIsotropicEquilateralParallelohedron : MonoBehaviour, IDamageable
{
    public event System.Action OnDeath;

    [Header("Enemy Stats")]
    public int maxHealth = 10000;
    private int currentHealth;
    public int playerleveltobossspawn = 2;

    [Header("XP Drop")]
    public GameObject[] xpPrefab;
    public int baseXP = 1000;
    public int enemyLevel = 20;

    [Header("Combat")]
    public int attackDamage = 25;
    public float attackRange = 16f;
    public float attackCooldown = 1.5f;
    private float lastAttackTime = 0f;

    [Header("Movement")]
    public float moveSpeed = 5f;
    private Transform player;
    private PlayerStats playerHealth;

    [Header("HealthBar")]
    public BossHealthBar bossHealthBar;

    [Header("Effects")]
    public GameObject explosionEffect;

    [Header("Other")]
    int rubbleLifetime = 5;
    public GameObject rubble;

    [Header("Fire Shot Attack")]
    public GameObject fireShotPrefab;
    public float fireInterval = 1f;
    public List<Transform> firePoints;

    private float fireTimer = 0f;

    void Start()
    {
        currentHealth = maxHealth;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            playerHealth = playerObj.GetComponent<PlayerStats>();

            if (playerHealth != null)
            {
                enemyLevel = playerHealth.playerLevel + 2;
                Debug.Log($"enemyLevel: {enemyLevel}");
            }
        }

        if (bossHealthBar != null)
        {
            bossHealthBar.Show();
            bossHealthBar.SetBossName("Euclidean Isotropic Equilateral Parallelohedron");
            bossHealthBar.SetHealth(currentHealth, maxHealth);
            bossHealthBar.SetText(currentHealth, maxHealth);
        }
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // --------------------------------------
        // 🔄 Rotate ONLY on Y axis
        // --------------------------------------
        Vector3 lookDir = player.position - transform.position;
        lookDir.y = 0f; // Remove vertical tilt for cube boss

        if (lookDir.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(lookDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 5f);
        }

        // Stay upright
        Vector3 e = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(0f, e.y, 0f);

        // Move toward player
        if (distance > attackRange)
            transform.position += transform.forward * moveSpeed * Time.deltaTime;
        else
            AttackIfReady();

        // Fire attack timer
        fireTimer += Time.deltaTime;
        if (fireTimer >= fireInterval)
        {
            FireRandomShot();
            fireTimer = 0f;
        }
    }

    // -----------------------------
    // 🔥 Fully accurate fire aiming
    // -----------------------------
    void FireRandomShot()
    {
        if (fireShotPrefab == null || firePoints == null || firePoints.Count == 0)
            return;

        Transform firePoint = firePoints[Random.Range(0, firePoints.Count)];

        // Aiming in full 3D (fixes the “shoots over player” bug)
        Vector3 dir = (player.position - firePoint.position).normalized;

        firePoint.rotation = Quaternion.LookRotation(dir);

        Instantiate(fireShotPrefab, firePoint.position, firePoint.rotation);
    }

    void AttackIfReady()
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            playerHealth?.TakeDamage(attackDamage);
            lastAttackTime = Time.time;
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (bossHealthBar != null)
        {
            bossHealthBar.SetHealth(currentHealth, maxHealth);
            bossHealthBar.SetText(currentHealth, maxHealth);
        }

        if (currentHealth <= 0)
            BossDie();
    }

    void BossDie()
    {
        OnDeath?.Invoke();

        if (rubble != null)
        {
            for (int i = 0; i < 100; i++)
            {
                Vector3 randomOffset = new Vector3(Random.Range(-1f, 1f), 1f, Random.Range(-1f, 1f));
                GameObject piece = Instantiate(rubble, transform.position + randomOffset, Quaternion.identity);
                Destroy(piece, rubbleLifetime);
            }
        }

        int totalXP = Mathf.RoundToInt(enemyLevel * 1) + baseXP;

        if (explosionEffect != null)
            Instantiate(explosionEffect, transform.position, Quaternion.identity);

        // 🔥 FIX: Hide boss HP bar on death
        if (bossHealthBar != null)
            bossHealthBar.Hide();

        Destroy(gameObject);
    }

}
