using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Header("Owned Wands")]
    public WandData[] availableWands;   // all wands the player owns
    private int currentWandIndex = 0;

    [Header("References")]
    public WandHolder wandHolder;        // reference to the WandHolder on the hand

    void Start()
    {
        if (availableWands.Length > 0)
            wandHolder.EquipWand(availableWands[currentWandIndex]);
    }

    void Update()
    {
        // Scroll wheel to switch wands
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f) NextWand();
        if (scroll < 0f) PreviousWand();

        // Number keys (optional)
        if (Input.GetKeyDown(KeyCode.Alpha1)) EquipWandAtIndex(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) EquipWandAtIndex(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) EquipWandAtIndex(2);
    }

    void NextWand()
    {
        if (availableWands.Length == 0) return;
        currentWandIndex = (currentWandIndex + 1) % availableWands.Length;
        wandHolder.EquipWand(availableWands[currentWandIndex]);
    }

    void PreviousWand()
    {
        if (availableWands.Length == 0) return;
        currentWandIndex--;
        if (currentWandIndex < 0) currentWandIndex = availableWands.Length - 1;
        wandHolder.EquipWand(availableWands[currentWandIndex]);
    }

    void EquipWandAtIndex(int index)
    {
        if (index < 0 || index >= availableWands.Length) return;
        currentWandIndex = index;
        wandHolder.EquipWand(availableWands[currentWandIndex]);
    }
}
