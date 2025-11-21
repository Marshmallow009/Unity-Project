using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class BossManager : MonoBehaviour
{
    [System.Serializable]
    public class BossSpawnData
    {
        public GameObject bossPrefab;
        public Transform spawnPoint;
        public int requiredLevel;
        [HideInInspector] public bool spawned = false;
    }

    public List<BossSpawnData> bosses = new List<BossSpawnData>();
    public PlayerStats playerStats;
    public BossHealthBar bossHealthBar;

    [Header("Warning UI")]
    public TextMeshProUGUI warningUI;
    public TextMeshProUGUI BossspawingUI;
    public float flashDuration = 1f;
    public int flashCount = 3;

    void Start()
    {
        if (playerStats == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                playerStats = playerObj.GetComponent<PlayerStats>();
        }

        // Make sure warning text is hidden at start
        if (warningUI != null)
            warningUI.gameObject.SetActive(false);

        if (BossspawingUI != null)
            BossspawingUI.gameObject.SetActive(false);
    }

    void Update()
    {
        if (playerStats == null) return;

        foreach (var bossData in bosses)
        {
            if (!bossData.spawned && playerStats.playerLevel >= bossData.requiredLevel)
            {
                bossData.spawned = true;
                StartCoroutine(FlashWarningThenSpawn(bossData));
            }
        }
    }

    IEnumerator FlashWarningThenSpawn(BossSpawnData bossData)
    {
        if (warningUI != null)
        {
            for (int i = 0; i < flashCount; i++)
            {
                // Show the warning
                warningUI.gameObject.SetActive(true);
                yield return new WaitForSeconds(flashDuration);

                // Hide the warning
                warningUI.gameObject.SetActive(false);
                yield return new WaitForSeconds(flashDuration);
            }
                BossspawingUI.gameObject.SetActive(true);
                yield return new WaitForSeconds(flashDuration);
                BossspawingUI.gameObject.SetActive(false);
                yield return new WaitForSeconds(flashDuration);
        }

        // Spawn the boss after flashing
        GameObject spawnedBoss = Instantiate(bossData.bossPrefab, bossData.spawnPoint.position, bossData.spawnPoint.rotation);
        EuclideanIsotropicEquilateralParallelohedron bossScript = spawnedBoss.GetComponent<EuclideanIsotropicEquilateralParallelohedron>();

        if (bossScript != null)
        {
            bossScript.bossHealthBar = bossHealthBar;
            bossHealthBar.Show();
            bossHealthBar.SetBossName(spawnedBoss.name);
            bossHealthBar.SetHealth(bossScript.maxHealth, bossScript.maxHealth);
            bossHealthBar.SetText(bossScript.maxHealth, bossScript.maxHealth);
        }
    }
}
