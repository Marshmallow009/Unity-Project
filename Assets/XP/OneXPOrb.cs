using UnityEngine;

public class OneXPOrb : MonoBehaviour
{
    [Header("XP Settings")]
    public int xpValue = 1;

    [Header("Magnet Settings")]
    public float pickupDistance = 5f; // Distance at which magnet activates
    public float magnetSpeed = 6f;    // Speed at which orb moves toward player

    private Transform player;
    private PlayerStats playerStats;
    private bool magnetActive = false;

    void Start()
    {
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
        foreach (var other in GameObject.FindGameObjectsWithTag("XP"))
        {
            if (other != gameObject)
            {
                Collider c1 = GetComponent<Collider>();
                Collider c2 = other.GetComponent<Collider>();
                if (c1 != null && c2 != null)
                    Physics.IgnoreCollision(c1, c2);
            }
        }
        // Find player object by tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj == null)
        {
            Debug.LogError("❌ Player not found! Make sure your Player has the tag 'Player'.");
            return;
        }

        player = playerObj.transform;
        playerStats = playerObj.GetComponent<PlayerStats>();

        if (playerStats == null)
        {
            Debug.LogError("❌ PlayerStats component not found on Player!");
        }
    }

    void Update()
    {
        if (player == null || playerStats == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // Activate magnet if within pickup distance
        if (!magnetActive && distance <= pickupDistance)
        {
            magnetActive = true;
        }

        if (magnetActive)
        {
            // Move orb smoothly toward the player
            transform.position = Vector3.MoveTowards(
                transform.position,
                player.position,
                magnetSpeed * Time.deltaTime
            );

            // Give XP and destroy orb if very close
            if (distance < 3f)
            {
                playerStats.AddXP(xpValue);
                Destroy(gameObject);
            }
        }
    }
}
