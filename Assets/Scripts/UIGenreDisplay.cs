using UnityEngine;
using TMPro;

public class UIGenreDisplay : MonoBehaviour
{
    [Header("--- Genre Count Display References ---")]
    [SerializeField] private TextMeshProUGUI crimeCountText;
    [SerializeField] private TextMeshProUGUI dramaCountText;
    [SerializeField] private TextMeshProUGUI factCountText;
    [SerializeField] private TextMeshProUGUI fantasyCountText;
    [SerializeField] private TextMeshProUGUI classicCountText;
    [SerializeField] private TextMeshProUGUI kidsCountText;
    [SerializeField] private TextMeshProUGUI travelCountText;

    void Update()
    {
        if (StorageManager.Instance == null) return;

        crimeCountText.text = StorageManager.Instance.bookStorage["Crime"].ToString();
        dramaCountText.text = StorageManager.Instance.bookStorage["Drama"].ToString();
        factCountText.text = StorageManager.Instance.bookStorage["Fact"].ToString();
        fantasyCountText.text = StorageManager.Instance.bookStorage["Fantasy"].ToString();
        classicCountText.text = StorageManager.Instance.bookStorage["Classic"].ToString();
        kidsCountText.text = StorageManager.Instance.bookStorage["Kids"].ToString();
        travelCountText.text = StorageManager.Instance.bookStorage["Travel"].ToString();
    }
}
