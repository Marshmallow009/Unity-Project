using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyHealthBar : MonoBehaviour
{
    [Header("UI")]
    public Image healthBarFill;
    public TextMeshProUGUI healthText;

    private Transform enemyTransform;
    private int maxHealth;
    private int currentHealth;
    private float heightOffset;

    public void Initialize(Transform enemy, int maxHP, float offset)
    {
        enemyTransform = enemy;
        maxHealth = maxHP;
        currentHealth = maxHP;
        heightOffset = offset;

        // Parent to enemy
        transform.SetParent(enemyTransform);
        transform.localPosition = new Vector3(0, heightOffset, 0);
        UpdateHealthUI();
    }

    void LateUpdate()
    {
        if (Camera.main != null)
        {
            // Make health bar face camera
            transform.LookAt(Camera.main.transform);
            transform.Rotate(0, 180f, 0); // flip if needed
        }
    }

    public void UpdateHealth(int newHealth)
    {
        currentHealth = newHealth;
        UpdateHealthUI();
    }

    void UpdateHealthUI()
    {
        if (healthBarFill != null)
            healthBarFill.fillAmount = (float)currentHealth / maxHealth;
        if (healthText != null)
            healthText.text = $"{currentHealth}/{maxHealth}";
    }
}
