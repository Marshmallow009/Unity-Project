using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossHealthBar : MonoBehaviour
{
    public Image bossHealthBarImage;
    public TextMeshProUGUI bossNameText;
    public TextMeshProUGUI bossHealthBarText;
    public GameObject bossStuff;

    void Awake()
    {
        // Ensure the boss UI is hidden at the start
        if (bossStuff != null)
            bossStuff.SetActive(false);
    }

    public void SetBossName(string name)
    {
        bossNameText.text = name;
    }
    public void SetText(int currentHealth, int maxHealth)
    {
        bossHealthBarText.text = $"{currentHealth}/{maxHealth}";
    }
    public void SetHealth(int currentHealth, int maxHealth)
    {
        bossHealthBarImage.fillAmount= (float)currentHealth / maxHealth;
    }

    public void Show()
    {
        if (bossStuff != null)
            bossStuff.SetActive(true);
    }

    public void Hide()
    {
        if (bossStuff != null)
            bossStuff.SetActive(false);
    }

}
