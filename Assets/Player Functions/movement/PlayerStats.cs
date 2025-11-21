using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStats : MonoBehaviour
{
    [Header("Player Stats")]
    public int maxHealth = 100;
    public int maxMana = 100;
    public int currentPlayerHealth;
    public int currentMana;
    public int healthRegenRate = 1;
    public int manaRegenRate = 5;
    public int baseXP = 100;

    [Header("UI")]
    public Image healthBarFill;
    public TextMeshProUGUI healthText;
    public Image ManaBarFill;
    public TextMeshProUGUI ManaText;
    public Image levelBarFill;
    public TextMeshProUGUI levelText;

    private float healthRegenTimer = 0f;
    private float manaRegenTimer = 0f;

    [Header("Leveling")]
    public int playerLevel = 1;
    public int currentXP = 0;
    public int xpToNextLevel = 100;

    void Start()
    {
        currentPlayerHealth = maxHealth;
        currentMana = maxMana;

        UpdateHealthUI();
        UpdateManaUI();
        LevelUI();
    }

    void Update()
    {
        RegenerateHealth();
        RegenerateMana();
    }

    // ------------------------------------------------------------------
    // HEALTH
    // ------------------------------------------------------------------
    public void TakeDamage(int damage)
    {
        // FIRST: Check if shield absorbs the damage
        if (Shield.activeShield != null)
        {
            Shield.activeShield.AbsorbDamage(damage);
            return; // Shield absorbs everything
        }

        // If NO shield, damage player normally
        currentPlayerHealth -= damage;
        if (currentPlayerHealth < 0) currentPlayerHealth = 0;

        UpdateHealthUI();

        if (currentPlayerHealth <= 0)
            Die();
    }

    void RegenerateHealth()
    {
        healthRegenTimer += Time.deltaTime;
        if (healthRegenTimer >= 1f)
        {
            currentPlayerHealth = Mathf.Min(currentPlayerHealth + healthRegenRate, maxHealth);
            UpdateHealthUI();
            healthRegenTimer = 0f;
        }
    }

    void UpdateHealthUI()
    {
        if (healthBarFill != null)
            healthBarFill.fillAmount = (float)currentPlayerHealth / maxHealth;

        if (healthText != null)
            healthText.text = $"{currentPlayerHealth}/{maxHealth}";
    }

    // ------------------------------------------------------------------
    // MANA
    // ------------------------------------------------------------------
    public void UseMana(int amount)
    {
        currentMana -= amount;
        if (currentMana < 0) currentMana = 0;

        UpdateManaUI();
    }

    public bool SpendMana(float amount)
    {
        int manaCost = Mathf.RoundToInt(amount);

        if (currentMana >= manaCost)
        {
            currentMana -= manaCost;
            UpdateManaUI();
            return true;
        }

        return false;
    }

    void RegenerateMana()
    {
        manaRegenTimer += Time.deltaTime;
        if (manaRegenTimer >= 1f)
        {
            currentMana = Mathf.Min(currentMana + manaRegenRate, maxMana);
            UpdateManaUI();
            manaRegenTimer = 0f;
        }
    }

    void UpdateManaUI()
    {
        if (ManaBarFill != null)
            ManaBarFill.fillAmount = (float)currentMana / maxMana;

        if (ManaText != null)
            ManaText.text = $"{currentMana}/{maxMana}";
    }

    // ------------------------------------------------------------------
    // XP + LEVELING
    // ------------------------------------------------------------------
    public void AddXP(int amount)
    {
        currentXP += amount;
        LevelUI();

        if (currentXP >= xpToNextLevel)
            LevelUp();
    }

    void LevelUp()
    {
        playerLevel++;
        currentXP -= xpToNextLevel;
        xpToNextLevel = Mathf.RoundToInt(baseXP * Mathf.Pow(1.315f, playerLevel - 1));

        maxHealth += Mathf.RoundToInt(2.4765f * playerLevel);
        maxMana += Mathf.RoundToInt(1.375f * playerLevel);

        LevelUI();
    }

    void LevelUI()
    {
        if (levelBarFill != null)
            levelBarFill.fillAmount = (float)currentXP / xpToNextLevel;

        if (levelText != null)
            levelText.text = $"Player Level {playerLevel}: {currentXP}/{xpToNextLevel}";
    }

    void Die()
    {
        Debug.Log("Player died!");
        // respawn logic here
    }
}
