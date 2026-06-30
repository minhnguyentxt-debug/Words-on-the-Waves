using UnityEngine;

public class CargoSystem : MonoBehaviour
{
    public static CargoSystem Instance { get; private set; }

    // Cần tham chiếu tới DataManager từ Tuần 1 để lấy dữ liệu cấu hình JSON
    [SerializeField] private DataManager dataManager;

    // Lưu thể loại sách được chọn từ UI để sử dụng khi bấm phím 2
    private string selectedGenreToPlace = "";

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Hàm xử lý khi Người chơi bấm nút Mua Kiện hàng (Crate) từ UI
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            PurchaseCrate("Second-hand Crate"); // Tên phải khớp chính xác với file JSON
        }

        // Phím 2: Đặt sách thể loại được chọn từ UI
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            PlaceBookWithSelectedGenre();
        }
    }

    // Phương thức được gọi từ UI để cập nhật thể loại sách được chọn
    public void SetSelectedGenre(string genre)
    {
        selectedGenreToPlace = genre;
        Debug.Log($"[CargoSystem] Đã chọn thể loại sách: <color=yellow>{genre}</color>");
    }

    // Phương thức bắt đầu đặt sách khi bấm phím 2
    private void PlaceBookWithSelectedGenre()
    {
        if (string.IsNullOrEmpty(selectedGenreToPlace))
        {
            Debug.LogWarning("[CargoSystem] Chưa chọn thể loại sách! Vui lòng chọn từ UI trước.");
            return;
        }

        if (BookPlacementManager.Instance == null)
        {
            Debug.LogError("[CargoSystem] BookPlacementManager không tìm thấy!");
            return;
        }

        BookPlacementManager.Instance.StartDraggingBook(selectedGenreToPlace);
        Debug.Log($"[CargoSystem] Bắt đầu đặt sách thể loại: <color=green>{selectedGenreToPlace}</color>");
    }
    public void PurchaseCrate(string crateNameTarget)
    {
        if (dataManager == null || dataManager.gameData == null)
        {
            Debug.LogError("[CargoSystem] Chưa nạp hoặc thiếu cấu hình JSON!");
            return;
        }

        // 1. Tìm đúng kiện hàng trong cấu hình dữ liệu JSON
        CargoData targetCrate = dataManager.gameData.cargoList.Find(c => c.cargoName == crateNameTarget);

        if (targetCrate == null)
        {
            Debug.LogError($"[CargoSystem] Không tìm thấy kiện hàng nào có tên: {crateNameTarget}");
            return;
        }

        // 2. Tiến hành check ví và trừ tiền thông qua StorageManager
        if (StorageManager.Instance.DeductMoney(targetCrate.price))
        {
            Debug.Log($"[CargoSystem] Mua thành công kiện hàng: {targetCrate.crateName}. Tiến hành khui sách...");
            UnboxCrate(targetCrate);
        }
    }

    // 3. Thuật toán mở kiện hàng dựa trên tỷ lệ phần trăm (Weighted Random) từ JSON
    private void UnboxCrate(CargoData crate)
    {
        // Giả sử mỗi kiện hàng mua được sẽ mở ra cố định 10 cuốn sách ngẫu nhiên theo tỷ lệ drop rate
        int totalBooksToSpawn = 10;

        for (int i = 0; i < totalBooksToSpawn; i++)
        {
            string chosenGenre = GetRandomGenreBasedOnRate(crate);
            // Thêm cuốn sách vừa quay được vào kho lưu trữ
            StorageManager.Instance.AddBooksToStorage(chosenGenre, 1);
        }

        // In tổng số lượng tồn kho ra để kiểm tra kết quả
        StorageManager.Instance.LogStorageStatus();
    }

    // Hàm bổ trợ tính toán tỷ lệ phần trăm ngẫu nhiên
    private string GetRandomGenreBasedOnRate(CargoData crate)
    {
        float randomRoll = Random.Range(0f, 100f);
        float cumulativeRate = 0f;

        foreach (var dropRate in crate.dropRates)
        {
            cumulativeRate += dropRate.rate;
            if (randomRoll <= cumulativeRate)
            {
                return dropRate.genre;
            }
        }

        // Trường hợp khẩn cấp nếu điền JSON thiếu hụt tỷ lệ (Ví dụ mới tổng được 90%)
        return crate.dropRates[0].genre;
    }
}