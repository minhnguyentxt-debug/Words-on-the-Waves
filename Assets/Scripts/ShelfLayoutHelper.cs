using UnityEngine;

public class ShelfLayoutHelper : MonoBehaviour
{
    [Header("Layout Configuration")]
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private Transform shelf1Container;
    [SerializeField] private Transform shelf2Container;

    [Header("Grid Settings")]
    [SerializeField] private int rows = 2;
    [SerializeField] private int columns = 16;
    [SerializeField] private float spacingX = 0.3f;
    [SerializeField] private float spacingY = 0.8f;

    [ContextMenu("Generate Shelves with Layout")]
    public void GenerateShelvesWithLayout()
    {
        if (slotPrefab == null || shelf1Container == null || shelf2Container == null)
        {
            Debug.LogError("Missing references!");
            return;
        }

        GenerateShelf(shelf1Container, "Shelf1", 1);
        GenerateShelf(shelf2Container, "Shelf2", 2);
    }

    private void GenerateShelf(Transform container, string shelfName, int shelfNumber)
    {
        ClearContainer(container);

        WagonSlot[] slots = new WagonSlot[rows * columns];
        int slotIndex = 0;

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                Vector3 position = new Vector3(col * spacingX, row * spacingY, 0);
                GameObject slotObject = Instantiate(slotPrefab, container);
                slotObject.name = shelfName + "_Slot_" + row + "_" + col;
                slotObject.transform.localPosition = position;

                WagonSlot wagonSlot = slotObject.GetComponent<WagonSlot>();
                if (wagonSlot != null)
                {
                    slots[slotIndex] = wagonSlot;
                }
                slotIndex++;
            }
        }

        RegisterSlots(shelfNumber, slots);
        Debug.Log("Generated " + (rows * columns) + " slots for " + shelfName);
    }

    private void ClearContainer(Transform container)
    {
        while (container.childCount > 0)
        {
            DestroyImmediate(container.GetChild(0).gameObject);
        }
    }

    private void RegisterSlots(int shelfNumber, WagonSlot[] slots)
    {
        if (StorageManager.Instance == null)
        {
            Debug.LogError("StorageManager not initialized!");
            return;
        }

        if (shelfNumber == 1)
            StorageManager.Instance.shelf1.slots = slots;
        else if (shelfNumber == 2)
            StorageManager.Instance.shelf2.slots = slots;
    }
}
