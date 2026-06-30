using System.Collections.Generic;
using UnityEngine;

public class StorageManager : MonoBehaviour
{
    public static StorageManager Instance { get; private set; }

    [Header("--- Player Wallet ---")]
    public int currentMoney = 2000;

    [Header("--- Storage (Kho chứa) ---")]
    public Dictionary<string, int> bookStorage = new Dictionary<string, int>()
    {
        { "Crime", 0 }, { "Drama", 0 }, { "Fact", 0 },
        { "Fantasy", 0 }, { "Classic", 0 }, { "Kids", 0 }, { "Travel", 0 }
    };

    [Header("--- Shelf System ---")]
    public Shelf shelf1 = new Shelf();
    public Shelf shelf2 = new Shelf();

    public class Shelf
    {
        public int currentBooks = 0;
        public const int MAX_CAPACITY = 32;
        public WagonSlot[] slots = new WagonSlot[32];
    }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void AddBooksToStorage(string genre, int amount)
    {
        if (bookStorage.ContainsKey(genre))
        {
            bookStorage[genre] += amount;
            Debug.Log($"[KHO] Added {amount} books of {genre}. Storage: {bookStorage[genre]}");
        }
    }

    public bool DeductMoney(int amount)
    {
        if (currentMoney >= amount)
        {
            currentMoney -= amount;
            Debug.Log($"[VÍ] Deducted {amount}$. Remaining: {currentMoney}$");
            return true;
        }
        Debug.LogWarning("[VÍ] Not enough money!");
        return false;
    }

    public int GetTotalBooksOnShelves()
    {
        return shelf1.currentBooks + shelf2.currentBooks;
    }

    public int GetTotalInStorage()
    {
        int total = 0;
        foreach (var count in bookStorage.Values)
            total += count;
        return total;
    }

    public Shelf GetAvailableShelf()
    {
        if (shelf1.currentBooks < Shelf.MAX_CAPACITY) return shelf1;
        if (shelf2.currentBooks < Shelf.MAX_CAPACITY) return shelf2;
        return null;
    }

    public void LogStorageStatus()
    {
        string status = "--- STORAGE STATUS ---\n";
        foreach (var item in bookStorage)
            status += $"{item.Key}: {item.Value} | ";
        status += $"\nShelf 1: {shelf1.currentBooks}/32 | Shelf 2: {shelf2.currentBooks}/32";
        Debug.Log(status);
    }
}
