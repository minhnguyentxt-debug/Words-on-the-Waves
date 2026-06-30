using UnityEngine;

public class ShelfGridGenerator : MonoBehaviour
{
    [Header("--- Shelf Containers ---")]
    [SerializeField] private Transform shelf1Container;
    [SerializeField] private Transform shelf2Container;

    [Header("--- Slot Prefab ---")]
    [SerializeField] private GameObject slotPrefab;

    [Header("--- Layout Config ---")]
    private const int ROWS = 2;
    private const int BOOKS_PER_ROW = 16;
    private const float SPACING_X = 0.3f;
    private const float SPACING_Y = 0.8f;

    [ContextMenu("Generate Both Shelves (32 slots each)")]
    public void GenerateBothShelves()
    {
        GenerateShelf(shelf1Container, 1);
        GenerateShelf(shelf2Container, 2);
    }

    private void GenerateShelf(Transform container, int shelfNumber)
    {
        if (container == null)
        {
            Debug.LogError($"Shelf {shelfNumber} Container not assigned!");
            return;
        }

        // Clear existing slots
        while (container.childCount > 0)
        {
            DestroyImmediate(container.GetChild(0).gameObject);
        }

        // Generate 32 slots (2 rows × 16 books)
        WagonSlot[] slots = new WagonSlot[32];
        int slotIndex = 0;

        for (int y = 0; y < ROWS; y++)
        {
            for (int x = 0; x < BOOKS_PER_ROW; x++)
            {
                Vector3 pos = new Vector3(x * SPACING_X, y * SPACING_Y, 0);
                GameObject newSlot = Instantiate(slotPrefab, container);
                newSlot.transform.localPosition = pos;
                newSlot.name = $"Shelf{shelfNumber}_Slot_{y}_{x}";

                WagonSlot wagonSlot = newSlot.GetComponent<WagonSlot>();
                if (wagonSlot != null)
                {
                    slots[slotIndex] = wagonSlot;
                }
                slotIndex++;
            }
        }

        // Register slots with StorageManager
        if (shelfNumber == 1)
            StorageManager.Instance.shelf1.slots = slots;
        else if (shelfNumber == 2)
            StorageManager.Instance.shelf2.slots = slots;

        Debug.Log($"Generated 32 slots for Shelf {shelfNumber}");
    }
}
