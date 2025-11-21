using UnityEngine;
using System.Collections.Generic;

public class OHKO : MonoBehaviour, IMagicSpell
{
    [Header("Bullet Settings")]
    public int damage = 10;
    public float speed = 20f;
    public float lifetime = 5f;
    public int piercing = 5;
    private int remainingPierce;

    public float cooldown = 0.25f;
    public WandData equippedWand; // Assigned from Magic script

    [Header("Spell Settings")]
    public int costOfMana = 1;
    public int ManaCost => costOfMana;
    public float Cooldown => cooldown;

    [Header("Trail Settings")]
    public GameObject trailPrefab; // assign your trail prefab here
    private GameObject activeTrail; // stores the spawned trail reference
  
    private Vector3 moveDirection;
    private HashSet<GameObject> hitEnemies = new HashSet<GameObject>();

    void Start()
    {
        remainingPierce = piercing;
        Destroy(gameObject, lifetime);

        // Ignore collisions with other projectiles
        Collider myCollider = GetComponent<Collider>();
        foreach (var other in GameObject.FindGameObjectsWithTag("Projectile"))
        {
            if (other != gameObject)
            {
                Collider otherCollider = other.GetComponent<Collider>();
                if (otherCollider != null && myCollider != null)
                    Physics.IgnoreCollision(myCollider, otherCollider);
            }
        }

        // ✅ Instantiate and attach trail
        if (trailPrefab != null)
        {
            activeTrail = Instantiate(trailPrefab, transform.position, Quaternion.identity, transform);
        }

        // make sure collider is trigger
        if (myCollider != null)
            myCollider.isTrigger = true;
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
        if (other.CompareTag("Projectile") || other.CompareTag("XP"))
            return;

        if (hitEnemies.Contains(other.gameObject))
            return;

        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            // Apply wand multiplier if available
            int finalDamage = damage;
            if (equippedWand != null)
                finalDamage = Mathf.RoundToInt(damage * equippedWand.magicPowerMultiplier);

            damageable.TakeDamage(finalDamage);
            hitEnemies.Add(other.gameObject);
            remainingPierce--;

            Debug.Log($"Bullet hit {other.name} for {finalDamage} damage using wand {equippedWand?.wandName ?? "None"}");

            if (remainingPierce <= 0)
                Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        // ✅ Detach and fade trail naturally
        if (activeTrail != null)
        {
            activeTrail.transform.parent = null;
            TrailRenderer tr = activeTrail.GetComponent<TrailRenderer>();
            if (tr != null)
            {
                tr.emitting = false;
                tr.autodestruct = true;
            }
        }
    }
}
