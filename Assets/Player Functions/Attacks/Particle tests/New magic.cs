using UnityEngine;
using System.Collections.Generic;

public class MagicBulletexplosion : MonoBehaviour, IMagicSpell
{
    [Header("Bullet Settings")]
    public int damage = 10;
    public float speed = 20f;
    public float lifetime = 5f;
    public int piercing = 5;
    private int remainingPierce;

    public float cooldown = 0.25f;

    [Header("Spell Settings")]
    public int costOfMana = 1;
    public int ManaCost => costOfMana;
    public float Cooldown => cooldown;

    [Header("Explosion Settings")]
    public GameObject explosionEffect;
    public float explosionRadius = 5f;

    [HideInInspector] public WandData wandData;

    private Vector3 moveDirection;
    private HashSet<GameObject> hitEnemies = new HashSet<GameObject>();

    void Start()
    {
        remainingPierce = piercing;
        Destroy(gameObject, lifetime);

        Collider myCollider = GetComponent<Collider>();
        if (myCollider != null)
        {
            myCollider.isTrigger = true;

            // Ignore collisions with the player/wand to avoid immediate explosion
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                Collider playerCol = player.GetComponent<Collider>();
                if (playerCol != null)
                    Physics.IgnoreCollision(myCollider, playerCol);
            }
        }
    }

    void Update()
    {
        transform.position += moveDirection * speed * Time.deltaTime;
    }

    public void Cast(Vector3 targetPoint, Transform launchPoint)
    {
        transform.position = launchPoint.position;
        moveDirection = (targetPoint - launchPoint.position).normalized;
        transform.rotation = Quaternion.LookRotation(moveDirection);
    }

    void OnTriggerEnter(Collider other)
    {
        // Only trigger explosion when hitting enemies or environment
        if (other.CompareTag("Enemy") || other.CompareTag("Environment"))
        {
            ExplodeAndDamage();
            Destroy(gameObject);
        }
    }

    void ExplodeAndDamage()
    {
        // Spawn explosion effect
        if (explosionEffect != null)
            Instantiate(explosionEffect, transform.position, Quaternion.identity);

        // Apply AoE damage to all enemies in radius
        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Enemy") && !hitEnemies.Contains(hit.gameObject))
            {
                IDamageable damageable = hit.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    int finalDamage = Mathf.RoundToInt(damage * (wandData?.magicPowerMultiplier ?? 1f));
                    damageable.TakeDamage(finalDamage);
                    hitEnemies.Add(hit.gameObject);
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
