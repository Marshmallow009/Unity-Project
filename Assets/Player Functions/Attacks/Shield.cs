using UnityEngine;

public class Shield : MonoBehaviour, IMagicSpell
{
    [Header("Shield Settings")]
    public float lifetime = 10;                  // 0 = infinite
    public int maxShieldHP = 100;               // Total HP shield can absorb
    private int currentHP;

    public float cooldown = 1.5f;               // Cooldown
    public int costOfMana = 5;                  // 0 mana to cast
    public int ManaCost => costOfMana;
    public float Cooldown => cooldown;

    [Header("Shield Follow Offset")]
    public Vector3 followOffset = new Vector3(0, 1f, 1.5f);

    [HideInInspector] public WandData wandData;

    private PlayerStats playerStats;

    // Global toggle reference
    public static Shield activeShield;

    void Start()
    {
        currentHP = maxShieldHP;
        playerStats = FindAnyObjectByType<PlayerStats>();

        // Make sure shield never pushes the player
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.isTrigger = true;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;
            rb.isKinematic = true;
        }

        // Optional lifetime support
        if (lifetime > 0)
            Destroy(gameObject, lifetime);
    }

    void Update()
    {
        if (playerStats == null) return;

        Transform player = playerStats.transform;

        // Stay in front of player using offset relative to rotation
        transform.position =
            player.position
            + player.forward * followOffset.z
            + player.up * followOffset.y
            + player.right * followOffset.x;

        transform.rotation = player.rotation;
    }

    // --------------------------
    //  CAST (Toggle On / Off)
    // --------------------------
    public void Cast(Vector3 targetPoint, Transform launchPoint)
    {
        // If a shield exists → destroy it instead (toggle off)
        if (activeShield != null)
        {
            Destroy(activeShield.gameObject);
            activeShield = null;
            return;
        }

        // Toggle ON
        activeShield = this;

        playerStats = FindAnyObjectByType<PlayerStats>();
        currentHP = maxShieldHP;

        if (playerStats != null)
            transform.position = playerStats.transform.position;
    }

    // --------------------------
    //  SHIELD DAMAGE ABSORB
    // --------------------------
    public void AbsorbDamage(int dmg)
    {
        if (playerStats == null) return;

        // Mana cost = 10% of absorbed damage
        int manaCost = Mathf.RoundToInt(dmg * 0.25f);

        // UseMana already clamps, so check BEFORE calling it
        if (playerStats.currentMana < manaCost)
        {
            // Not enough mana → shield breaks
            DestroyShield();
            return;
        }

        // Spend mana
        playerStats.UseMana(manaCost);
    }


    void DestroyShield()
    {
        if (activeShield == this)
            activeShield = null;

        Destroy(gameObject);
    }
}
