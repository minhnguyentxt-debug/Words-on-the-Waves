using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIGenreSelector : MonoBehaviour
{
    [Header("Genre Buttons")]
    [SerializeField] private Button crimeButton;
    [SerializeField] private Button dramaButton;
    [SerializeField] private Button factButton;
    [SerializeField] private Button fantasyButton;
    [SerializeField] private Button classicButton;
    [SerializeField] private Button kidsButton;
    [SerializeField] private Button travelButton;

    [Header("Visual Feedback")]
    [SerializeField] private TextMeshProUGUI selectedGenreDisplay;
    [SerializeField] private Color selectedButtonColor = new Color(0.2f, 0.8f, 0.2f);
    [SerializeField] private Color normalButtonColor = new Color(1f, 1f, 1f);

    private Button currentlySelectedButton = null;
    private CargoSystem cargoSystem;

    void Start()
    {
        cargoSystem = CargoSystem.Instance;
        if (cargoSystem == null)
        {
            Debug.LogError("[UI] CargoSystem not found");
            return;
        }

        SetupButton(crimeButton, "Crime");
        SetupButton(dramaButton, "Drama");
        SetupButton(factButton, "Fact");
        SetupButton(fantasyButton, "Fantasy");
        SetupButton(classicButton, "Classic");
        SetupButton(kidsButton, "Kids");
        SetupButton(travelButton, "Travel");

        Debug.Log("[UI] UIGenreSelector ready");
    }

    private void SetupButton(Button button, string genre)
    {
        if (button == null)
        {
            Debug.LogWarning("[UI] Button " + genre + " not assigned");
            return;
        }
        button.onClick.AddListener(() => SelectGenre(genre, button));
    }

    private void SelectGenre(string genre, Button clickedButton)
    {
        if (cargoSystem == null) return;

        if (currentlySelectedButton == clickedButton)
        {
            DeselectButton(clickedButton, genre);
            return;
        }

        if (currentlySelectedButton != null)
        {
            StartCoroutine(ScaleButton(currentlySelectedButton, new Vector2(0.4f, 1.4f), 0.2f));
        }

        cargoSystem.SetSelectedGenre(genre);
        UpdateButtonVisuals(clickedButton);
        currentlySelectedButton = clickedButton;

        if (selectedGenreDisplay != null)
        {
            selectedGenreDisplay.text = "Selected: " + genre;
        }
        Debug.Log("[UI] Selected: " + genre);

        // 🔥 Immediately start book placement when genre is selected (no need for key 2)
        if (BookPlacementManager.Instance != null)
        {
            BookPlacementManager.Instance.StartDraggingBook(genre);
            Debug.Log("[UI] Started dragging book for genre: " + genre);
        }
    }

    private void DeselectButton(Button button, string genre)
    {
        StartCoroutine(ScaleButton(button, new Vector2(0.4f, 1.4f), 0.2f));
        cargoSystem.SetSelectedGenre("");
        currentlySelectedButton = null;

        if (selectedGenreDisplay != null)
        {
            selectedGenreDisplay.text = "Đã bỏ chọn: " + genre;
            StartCoroutine(HideTextAfterDelay(2f));
        }
    }

    private void UpdateButtonVisuals(Button currentButton)
    {
        StartCoroutine(ScaleButton(currentButton, new Vector2(0.6f, 1.8f), 0.2f));
    }

    private System.Collections.IEnumerator ScaleButton(Button button, Vector2 targetScaleXY, float duration)
    {
        Vector3 startScale = button.transform.localScale;
        Vector3 endScale = new Vector3(targetScaleXY.x, targetScaleXY.y, 1f);
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            button.transform.localScale = Vector3.Lerp(startScale, endScale, t);
            yield return null;
        }

        button.transform.localScale = endScale;
    }

    private System.Collections.IEnumerator HideTextAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (selectedGenreDisplay != null && currentlySelectedButton == null)
        {
            selectedGenreDisplay.text = "";
        }
    }
}
