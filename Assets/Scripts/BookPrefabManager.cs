using UnityEngine;
using System.Collections.Generic;

public class BookPrefabManager : MonoBehaviour
{
    public static BookPrefabManager Instance { get; private set; }

    [Header("---     Book Prefabs By Genre ---")]
    [SerializeField] private GameObject crimePrefab;
    [SerializeField] private GameObject dramaPrefab;
    [SerializeField] private GameObject factPrefab;
    [SerializeField] private GameObject fantasyPrefab;
    [SerializeField] private GameObject classicPrefab;
    [SerializeField] private GameObject kidsPrefab;
    [SerializeField] private GameObject travelPrefab;

    private Dictionary<string, GameObject> bookPrefabs = new Dictionary<string, GameObject>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Map genres to prefabs
        bookPrefabs["Crime"] = crimePrefab;
        bookPrefabs["Drama"] = dramaPrefab;
        bookPrefabs["Fact"] = factPrefab;
        bookPrefabs["Fantasy"] = fantasyPrefab;
        bookPrefabs["Classic"] = classicPrefab;
        bookPrefabs["Kids"] = kidsPrefab;
        bookPrefabs["Travel"] = travelPrefab;

        Debug.Log("[BookPrefabManager] Initialized with " + bookPrefabs.Count + " book types");
    }

    /// <summary>
    /// Get book prefab for a specific genre
    /// </summary>
    public GameObject GetBookPrefab(string genre)
    {
        if (bookPrefabs.ContainsKey(genre))
        {
            return bookPrefabs[genre];
        }
        Debug.LogWarning("[BookPrefabManager] No prefab found for genre: " + genre);
        return null;
    }
}
