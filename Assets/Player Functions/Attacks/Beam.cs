using UnityEngine;

public class BeamSpell : MonoBehaviour, IMagicSpell
{
    public GameObject laserBeamPrefab;        // The prefab (cylinder/mesh)
    public float lifetime = 2f;               // Auto destroy timer
    public float cooldown = 0.2f;             // Spell cooldown
    public int costOfMana = 1;                // Cost per cast

    public int ManaCost => costOfMana;
    public float Cooldown => cooldown;

    public void Cast(Vector3 targetPoint, Transform launchPoint)
    {
        // Instantiate beam at wand/staff/etc.
        GameObject beam = Instantiate(laserBeamPrefab, launchPoint.position, launchPoint.rotation);

        // Aim beam toward crosshair
        Vector3 direction = (targetPoint - launchPoint.position).normalized;
        beam.transform.rotation = Quaternion.LookRotation(direction);

        // Destroy after lifetime
        Destroy(beam, lifetime);
    }
}

    // public GameObject projectilePrefab;
    // public float manaCostPerSecond = 10f;
    // public float fireRate = 0.05f; // shoots every 0.05 seconds (20 hits per second)

    // public int ManaCost => 0; // we drain per second instead
    // public float Cooldown => 0f; // continuous fire allowed

    // private PlayerStats playerStats;
    // private Transform launchPoint;
    // private float fireTimer = 0f;

    // public void Cast(Vector3 targetPoint, Transform launchPoint)
    // {
    //     this.launchPoint = launchPoint;
    //     playerStats = launchPoint.GetComponentInParent<PlayerStats>();
    //     fireTimer = 0f;
    // }

    // void Update()
    // {
    //     if (playerStats == null || launchPoint == null)
    //         return;

    //     // Drain mana over time
    //     float manaThisFrame = manaCostPerSecond * Time.deltaTime;
    //     if (playerStats.currentMana < manaThisFrame)
    //         return;

    //     playerStats.UseMana((int)manaThisFrame);

    //     // Fire repeatedly
    //     fireTimer -= Time.deltaTime;
    //     if (fireTimer <= 0f)
    //     {
    //         FireBeamRay();
    //         fireTimer = fireRate;
    //     }
    // }

    // void FireBeamRay()
    // {
    //     // Raycast to crosshair
    //     Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
    //     Vector3 target = ray.GetPoint(100f);

    //     if (Physics.Raycast(ray, out RaycastHit hit, 100f))
    //     {
    //         target = hit.point;
    //     }

    //     // Spawn projectile facing the hit point
    //     Vector3 direction = (target - launchPoint.position).normalized;
    //     GameObject proj = Instantiate(projectilePrefab, launchPoint.position, Quaternion.LookRotation(direction));

    //     // Because projectile script already handles damage, we’re done.
    // }
// }
