using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonDragHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private string genre;
    private bool isPointerDown = false;

    public void Setup(string genreType)
    {
        genre = genreType;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPointerDown = true;
        
        if (string.IsNullOrEmpty(genre))
        {
            Debug.LogWarning("Genre not set for ButtonDragHandler");
            return;
        }

        if (StorageManager.Instance.bookStorage[genre] <= 0)
        {
            Debug.LogWarning("No books of genre: " + genre);
            isPointerDown = false;
            return;
        }

        Debug.Log("Started dragging: " + genre);
        BookPlacementManager.Instance.StartDraggingBook(genre);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPointerDown = false;
        Debug.Log("Released drag");
    }
}
