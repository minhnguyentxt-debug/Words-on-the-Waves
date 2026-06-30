using UnityEngine;
using TMPro;

public class ShelfDisplayUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI inStorageText;
    [SerializeField] private TextMeshProUGUI shelf1Text;
    [SerializeField] private TextMeshProUGUI shelf2Text;
    [SerializeField] private TextMeshProUGUI totalInShopText;

    void Update()
    {
        if (StorageManager.Instance == null) return;

        int totalStorage = StorageManager.Instance.GetTotalInStorage();
        int shelf1Count = StorageManager.Instance.shelf1.currentBooks;
        int shelf2Count = StorageManager.Instance.shelf2.currentBooks;
        int totalShop = shelf1Count + shelf2Count;

        if (inStorageText != null)
            inStorageText.text = "In Storage: " + totalStorage;

        if (shelf1Text != null)
            shelf1Text.text = "Shelf 1: " + shelf1Count + "/32";

        if (shelf2Text != null)
            shelf2Text.text = "Shelf 2: " + shelf2Count + "/32";

        if (totalInShopText != null)
            totalInShopText.text = "In Shop: " + totalShop + "/64";
    }
}
