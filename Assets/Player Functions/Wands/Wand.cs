using UnityEngine;

[CreateAssetMenu(fileName = "NewWand", menuName = "Magic/Wand")]
public class WandData : ScriptableObject
{
    [Header("Wand Info")]
    public string wandName = "New Wand";
    public GameObject wandModel; // Prefab of the wand

    [Header("Stat Modifiers")]
    [Tooltip("Scales spell damage (1 = normal, 2 = double, etc.)")]
    public float magicPowerMultiplier = 1f;

    [Tooltip("Scales cooldown (1 = normal, 0.8 = faster, 1.2 = slower)")]
    public float cooldownMultiplier = 1f;

    [Tooltip("Bonus mana regen per second while equipped")]
    public float manaRegenBonus = 0f;
}
