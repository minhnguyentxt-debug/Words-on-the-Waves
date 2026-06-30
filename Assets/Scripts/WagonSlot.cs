using UnityEngine;

public class WagonSlot : MonoBehaviour
{
    [Header("Slot Status")]
    public bool isOccupied = false;
    public string currentBookGenre = "";

    [Header("Visual Reference")]
    [SerializeField] private GameObject realBookVisual;

    void Awake()
    {
        if (realBookVisual != null) realBookVisual.SetActive(false);
    }

    public void PlaceBook(string genre)
    {
        isOccupied = true;
        currentBookGenre = genre;

        // Destroy old visual
        if (realBookVisual != null)
        {
            Destroy(realBookVisual.gameObject);
            realBookVisual = null;
        }

        // Get correct book prefab for genre
        if (BookPrefabManager.Instance != null)
        {
            GameObject bookPrefab = BookPrefabManager.Instance.GetBookPrefab(genre);
            
            if (bookPrefab != null)
            {
                // Instantiate correct book model
                GameObject newBook = Instantiate(bookPrefab, transform);
                
                // Set scale
                newBook.transform.localScale = new Vector3(8f, 4f, 3.5f);
                
                // Set rotation
                newBook.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);
                
                // Set position
                newBook.transform.localPosition = new Vector3(0f, -0.5f, 0f);
                
                newBook.name = $"{genre}_Book";
                realBookVisual = newBook;
                
                Debug.Log($"[SLOT] Placed {genre} book model");
            }
            else
            {
                Debug.LogWarning($"[SLOT] Prefab not found for genre: {genre}");
            }
        }
        else
        {
            Debug.LogError("[SLOT] BookPrefabManager not found!");
        }

        // Update shelf count
        StorageManager.Shelf shelf = GetParentShelf();
        if (shelf != null) shelf.currentBooks++;
    }

    public void RemoveBook()
    {
        if (!isOccupied) return;

        isOccupied = false;
        currentBookGenre = "";
        
        // Destroy book model
        if (realBookVisual != null)
        {
            Destroy(realBookVisual.gameObject);
            realBookVisual = null;
        }

        StorageManager.Shelf shelf = GetParentShelf();
        if (shelf != null) shelf.currentBooks--;
    }

    private StorageManager.Shelf GetParentShelf()
    {
        if (transform.parent != null && transform.parent.parent != null)
        {
            Transform shelfContainer = transform.parent.parent;
            if (shelfContainer.name.Contains("Shelf1")) return StorageManager.Instance.shelf1;
            if (shelfContainer.name.Contains("Shelf2")) return StorageManager.Instance.shelf2;
        }
        return null;
    }
}
