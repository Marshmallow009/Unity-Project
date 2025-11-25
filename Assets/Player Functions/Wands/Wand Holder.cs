using UnityEngine;

public class WandHolder : MonoBehaviour
{
    public WandData equippedWand; // Assign the WandData ScriptableObject
    [HideInInspector] public Transform wandTip;
    private GameObject currentWandInstance;

    void Start()
    {
        if (equippedWand != null)
            EquipWand(equippedWand);
    }

    public void EquipWand(WandData newWand)
    {
        // Destroy old wand
        if (currentWandInstance != null)
            Destroy(currentWandInstance);

        equippedWand = newWand;
        if (equippedWand == null || equippedWand.wandModel == null) return;

        // Instantiate wand prefab as child
        currentWandInstance = Instantiate(equippedWand.wandModel, transform);

        // Apply hand offset
        currentWandInstance.transform.localPosition = equippedWand.handOffset;

        // Preserve rotation & scale
        // No changes needed

        // Find WandTip
        wandTip = currentWandInstance.transform.Find("WandTip");
        if (wandTip == null)
        {
            Debug.LogWarning("WandTip not found! Defaulting to wand root.");
            wandTip = currentWandInstance.transform;
        }
    }
}
