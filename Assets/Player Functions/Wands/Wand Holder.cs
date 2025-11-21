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
        if (currentWandInstance != null)
            Destroy(currentWandInstance);

        equippedWand = newWand;
        if (equippedWand == null || equippedWand.wandModel == null) return;

        // Instantiate wand model
        currentWandInstance = Instantiate(equippedWand.wandModel, transform);

        // Keep the prefab's rotation and scale, just reset position to hand
        currentWandInstance.transform.localPosition = Vector3.zero;

        // Find WandTip child
        wandTip = currentWandInstance.transform.Find("WandTip");
        if (wandTip == null)
        {
            Debug.LogWarning("WandTip not found! Defaulting to wand holder transform.");
            wandTip = transform;
        }
    }
}
