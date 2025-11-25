using UnityEngine;
using System.Collections;

public class Magic : MonoBehaviour
{
    [Header("Attack Settings")]
    public GameObject[] magicPrefabs;
    private int currentAttackIndex = 0;

    [Header("Projectile Options")]
    public int numOfProjectiles = 0;

    [Header("References")]
    public WandHolder wandHolder;
    public float launchForce = 20f;
    public float maxDistance = 100f;

    [Header("Camera")]
    public Camera activeCamera; // assign dynamically when switching perspectives

    [Header("Cooldown / Mana")]
    private bool canShoot = true;
    public PlayerStats playerMana;

    void Update()
    {
        if (wandHolder == null || wandHolder.wandTip == null) return;

        GameObject spellPrefab = magicPrefabs.Length > 0 ? magicPrefabs[currentAttackIndex] : null;
        if (spellPrefab == null) return;

        Transform launchPoint = wandHolder.wandTip;
        WandData equippedWand = wandHolder.equippedWand;

        // --------------------------------------------------
        // CAST on press C
        // --------------------------------------------------
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (spellPrefab.TryGetComponent<newLaserBeam>(out newLaserBeam laserSpell))
            {
                laserSpell.Cast(Vector3.zero, launchPoint);
            }
            else if (canShoot)
            {
                StartCoroutine(ShootMagicCooldown());
            }
        }

        // --------------------------------------------------
        // STOP LASER
        // --------------------------------------------------
        if (Input.GetKeyUp(KeyCode.C))
        {
            if (spellPrefab.TryGetComponent<newLaserBeam>(out newLaserBeam laserSpell))
            {
                laserSpell.StopBeam();
            }
        }

        // Swap attacks
        if (Input.GetKeyDown(KeyCode.O))
        {
            ChangeAttack();
        }
    }

    // --------------------------------------------------
    // SHOOTING COOLDOWN
    // --------------------------------------------------
    IEnumerator ShootMagicCooldown()
    {
        canShoot = false;

        GameObject spellPrefab = magicPrefabs[currentAttackIndex];
        IMagicSpell spellData = spellPrefab.GetComponent<IMagicSpell>();
        WandData equippedWand = wandHolder.equippedWand;

        if (spellData == null)
        {
            Debug.LogError("Prefab does not implement IMagicSpell: " + spellPrefab.name);
            canShoot = true;
            yield break;
        }

        // Mana check
        int finalManaCost = Mathf.RoundToInt(spellData.ManaCost * (equippedWand?.magicPowerMultiplier ?? 1f));
        if (playerMana.currentMana < finalManaCost)
        {
            Debug.Log("Not enough mana to cast.");
            canShoot = true;
            yield break;
        }

        playerMana.UseMana(finalManaCost);

        // Use currently active camera for raycasting
        if (activeCamera == null)
        {
            activeCamera = Camera.main;
        }

        Ray ray = activeCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Vector3 targetPoint = Physics.Raycast(ray, out RaycastHit hit, maxDistance) ? hit.point : ray.GetPoint(maxDistance);

        // Apply projectile offset from wand tip
        Vector3 spawnPosition = wandHolder.wandTip.position + (equippedWand?.projectileOffset ?? Vector3.zero);
        Vector3 direction = (targetPoint - spawnPosition).normalized;

        // Spell types
        if (spellData is TrippleShot)
            ShootTriple(spellPrefab, direction, equippedWand);
        else if (spellData is ShootAmount)
            ShootAmount(spellPrefab, direction, numOfProjectiles, equippedWand);
        else
            ShootSingle(spellPrefab, direction, equippedWand);

        // Apply cooldown
        float finalCooldown = spellData.Cooldown * (equippedWand?.cooldownMultiplier ?? 1f);
        yield return new WaitForSeconds(finalCooldown);
        canShoot = true;
    }

    // --------------------------------------------------
    // SHOOT SINGLE PROJECTILE
    // --------------------------------------------------
    void ShootSingle(GameObject spellPrefab, Vector3 direction, WandData equippedWand)
    {
        Vector3 spawnPosition = wandHolder.wandTip.position + (equippedWand?.projectileOffset ?? Vector3.zero);
        GameObject projectile = Instantiate(spellPrefab, spawnPosition, Quaternion.LookRotation(direction));

        MagicBullet bullet = projectile.GetComponent<MagicBullet>();
        if (bullet != null)
            bullet.wandData = equippedWand;

        if (projectile.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.linearVelocity = direction * launchForce;
            rb.useGravity = false;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }

        Destroy(projectile, 5f);
    }

    void ShootTriple(GameObject spellPrefab, Vector3 direction, WandData equippedWand)
    {
        ShootSingle(spellPrefab, direction, equippedWand);

        Vector3 left = Quaternion.Euler(0, -5f, 0) * direction;
        Vector3 right = Quaternion.Euler(0, 5f, 0) * direction;

        ShootSingle(spellPrefab, left, equippedWand);
        ShootSingle(spellPrefab, right, equippedWand);
    }

    void ShootAmount(GameObject spellPrefab, Vector3 direction, int numOfProjectiles, WandData equippedWand)
    {
        ShootSingle(spellPrefab, direction, equippedWand);

        int half = numOfProjectiles / 2;
        float angleStep = 5f;

        for (int i = 1; i <= half; i++)
        {
            float angle = angleStep * i;
            Vector3 leftDir = Quaternion.Euler(0, -angle, 0) * direction;
            Vector3 rightDir = Quaternion.Euler(0, angle, 0) * direction;

            ShootSingle(spellPrefab, leftDir, equippedWand);
            ShootSingle(spellPrefab, rightDir, equippedWand);
        }
    }

    void ChangeAttack()
    {
        if (magicPrefabs.Length <= 1) return;

        currentAttackIndex = (currentAttackIndex + 1) % magicPrefabs.Length;
        Debug.Log("Switched to: " + magicPrefabs[currentAttackIndex].name);
    }

    // --------------------------------------------------
    // Camera switch helper
    // --------------------------------------------------
    public void SetActiveCamera(Camera cam)
    {
        activeCamera = cam;
    }
}
