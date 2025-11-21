using UnityEngine;
using System.Collections.Generic;

public class MagicBullet : MonoBehaviour, IMagicSpell
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

    // ADD THIS:
    [HideInInspector] public WandData wandData;

    private Vector3 moveDirection;

    // Track which enemies this bullet has already hit
    private HashSet<GameObject> hitEnemies = new HashSet<GameObject>();

    void Start()
    {
        remainingPierce = piercing;
        Destroy(gameObject, lifetime);

        Collider myCollider = GetComponent<Collider>();
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
            // Use wandData multiplier
            int finalDamage = Mathf.RoundToInt(damage * (wandData?.magicPowerMultiplier ?? 1f));

            damageable.TakeDamage(finalDamage);
            hitEnemies.Add(other.gameObject);
            remainingPierce--;

            if (remainingPierce <= 0)
                Destroy(gameObject);
        }
    }
}
