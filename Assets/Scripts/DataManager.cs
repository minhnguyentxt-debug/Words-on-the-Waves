using System.IO;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    private string filePath;
    public CargoListWrapper gameData { get; private set; }

    void Awake()
    {
        // Đường dẫn trỏ tới thư mục StreamingAssets
        filePath = Path.Combine(Application.streamingAssetsPath, "CargoConfig.json");
        gameData = LoadCargoData();
    }

    // Hàm ĐỌC dữ liệu từ file JSON
    public CargoListWrapper LoadCargoData()
    {
        if (File.Exists(filePath))
        {
            string jsonText = File.ReadAllText(filePath);
            CargoListWrapper data = JsonUtility.FromJson<CargoListWrapper>(jsonText);
            Debug.Log("Đọc dữ liệu JSON thành công!");
            return data;
        }
        else
        {
            Debug.LogError("Không tìm thấy file JSON tại: " + filePath);
            return null;
        }
    }

    // Hàm GHI/CẬP NHẬT dữ liệu vào file JSON (Dùng khi muốn lưu trạng thái)
    public void SaveCargoData(CargoListWrapper data)
    {
        string jsonText = JsonUtility.ToJson(data, true); // "true" để tự động xuống dòng, dễ đọc
        File.WriteAllText(filePath, jsonText);
        Debug.Log("Đã lưu dữ liệu vào file JSON thành công!");
    }
}