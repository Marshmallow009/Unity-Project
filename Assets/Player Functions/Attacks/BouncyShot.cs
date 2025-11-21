using UnityEngine;
using System.Linq;

public class MagicBounceNoRB : MonoBehaviour, IMagicSpell
{
    [Header("Projectile Settings")]
    public int damage = 10;
    public float speed = 40f;
    public float lifetime = 5f;
    public int costOfMana = 1;
    public int maxBounces = 3;
    public float cooldown = 1.5f;

    public int ManaCost => costOfMana;
    public float Cooldown => cooldown;

    private int remainingBounces;
    private IDamageable currentTarget;

    void Start()
    {
        Destroy(gameObject, lifetime);

        // Ignore collisions with other projectiles
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
        remainingBounces = maxBounces;
        Destroy(gameObject, lifetime);
    }

    public void Cast(Vector3 targetPoint, Transform launchPoint)
    {
        transform.position = launchPoint.position;

        // Find initial target near the point you clicked
        currentTarget = FindClosestEnemy(targetPoint);
    }

    void OnCollisionEnter(Collision collision)
    {
        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damage);
            remainingBounces --;
        }
        if (remainingBounces == 0)
        {
            Destroy(gameObject);
        }

    }

    IDamageable FindClosestEnemy(Vector3 fromPosition)
    {
        IDamageable[] enemies = GameObject.FindObjectsOfType<MonoBehaviour>().OfType<IDamageable>().ToArray();
        IDamageable closest = null;
        float minDist = Mathf.Infinity;
        foreach (var enemy in enemies)
        {
            float dist = Vector3.Distance(fromPosition, ((MonoBehaviour)enemy).transform.position);
            if (dist < minDist && dist > 0.1f)
            {
                minDist = dist;
                closest = enemy;
            }
        }
        return closest;
    }

    Vector3 GetTargetPosition(IDamageable target)
    {
        return ((MonoBehaviour)target).transform.position;
    }
}
