using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class BookPlacementManager : MonoBehaviour
{
    public static BookPlacementManager Instance { get; private set; }

    [Header("--- Drag & Drop Config ---")]
    [SerializeField] private LayerMask slotLayerMask; // Tạo riêng 1 Layer tên là "Slots" và gán cho các WagonSlot để tối ưu Raycast
    [SerializeField] private float snapRadius = 1.5f;   // Bán kính để kích hoạt cơ chế hút tự động

    [Header("--- Ghost Mesh (Xem trước) ---")]
    // Kéo mô hình sách bán trong suốt (Gắn Material dạng Transparent) vào đây
    [SerializeField] private GameObject ghostMeshPrefab;

    private GameObject currentGhostInstance;
    private string selectedGenreToPlace = "";
    private bool isDragging = false;
    private Camera mainCamera;

    public GameObject tooltipPanel;
    public TextMeshProUGUI titleText; // Dùng kiểu TextMeshProUGUI thay vì Text
    public TextMeshProUGUI genreText;       // Kéo ô Text thể loại vào đây
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        mainCamera = Camera.main;
    }

    // HÀM KHỞI CHẠY (Sẽ được gọi từ UI khi người chơi nhấn giữ một loại sách từ Kho lưu trữ)
    public void StartDraggingBook(string genre)
    {
        Debug.Log("[DRAG] StartDraggingBook called with genre: " + genre);
        
        // Check if any shelf has available space (32 books max each)
        if (StorageManager.Instance.GetAvailableShelf() == null)
        {
            Debug.LogWarning("[SHELVES] Both shelves are full! Max 32 books each.");
            return;
        }

        // Check if storage has this book type
        if (StorageManager.Instance.bookStorage[genre] <= 0)
        {
            Debug.LogWarning($"[STORAGE] No more {genre} books available!");
            return;
        }

        selectedGenreToPlace = genre;
        isDragging = true;
        Debug.Log("[DRAG] isDragging = true, selectedGenre = " + genre);

        // Spawn Ghost Mesh at mouse position immediately
        if (ghostMeshPrefab != null)
        {
            Debug.Log("[DRAG] ghostMeshPrefab is NOT null - spawning...");
            currentGhostInstance = Instantiate(ghostMeshPrefab);
            currentGhostInstance.SetActive(true); // Visible immediately
            Debug.Log("[DRAG] Ghost mesh instantiated and activated");
            
            // Position at mouse
            Vector2 mousePos = Pointer.current.position.ReadValue();
            Ray ray = mainCamera.ScreenPointToRay(mousePos);
            currentGhostInstance.transform.position = ray.origin + ray.direction * 5f;
            Debug.Log("[DRAG] Ghost mesh positioned at: " + currentGhostInstance.transform.position);
        }
        else
        {
            Debug.LogError("[DRAG] ERROR: ghostMeshPrefab is NULL! Please assign in inspector!");
        }

        Debug.Log("[DRAG] Started dragging " + genre);
    }

    void Update()
    {
        if (!isDragging) return;
        
        Debug.Log("[UPDATE] isDragging=true, checking raycast...");

        // 1. Lấy vị trí con trỏ chuột hoặc điểm chạm màn hình theo New Input System
        Vector2 pointerPosition = Vector2.zero;
        if (Pointer.current != null)
        {
            pointerPosition = Pointer.current.position.ReadValue();
        }

        // 2. Bắn tia Physics.Raycast từ Camera xuống không gian 3D
        Ray ray = mainCamera.ScreenPointToRay(pointerPosition);
        Debug.DrawRay(ray.origin, ray.direction * 100, Color.red);
        RaycastHit hit;
        
        WagonSlot hoveredSlot = null;
        
        Debug.Log("[UPDATE] Current ghost instance: " + (currentGhostInstance != null ? "EXISTS" : "NULL!"));
        if (currentGhostInstance != null)
        {
            Renderer rend = currentGhostInstance.GetComponent<Renderer>();
            Debug.Log("[UPDATE] Ghost enabled: " + currentGhostInstance.activeSelf + ", Renderer enabled: " + (rend != null ? rend.enabled.ToString() : "NO_RENDERER"));
        }

        Debug.Log("[UPDATE] Ray origin: " + ray.origin + " | Direction: " + ray.direction);
        Debug.Log("[UPDATE] Raycast checking with LayerMask...");

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, slotLayerMask))
        {
            Debug.Log("[UPDATE] Raycast HIT something: " + hit.collider.gameObject.name);
            hoveredSlot = hit.collider.GetComponent<WagonSlot>();

            if (hoveredSlot != null && !hoveredSlot.isOccupied)
            {
                // Di chuyển Ghost Mesh bám theo vị trí của ô kệ đang rê qua (Preview Ghost Mesh)
                if (currentGhostInstance != null)
                {
                    currentGhostInstance.SetActive(true);
                    
                    // Set color to GREEN (slot is free)
                    Renderer ghostRenderer = currentGhostInstance.GetComponent<Renderer>();
                    if (ghostRenderer != null)
                    {
                        ghostRenderer.material.color = new Color(0.2f, 1f, 0.2f, 0.5f); // Light green
                    }
                    
                    // Lấy vị trí từ collider bounds center (chính xác hơn transform.position)
                    Collider slotCollider = hoveredSlot.GetComponent<Collider>();
                    if (slotCollider != null)
                    {
                        // 📍 Set position to slot center
                        currentGhostInstance.transform.position = slotCollider.bounds.center + new Vector3(0f, -0.2f, 0f);
                        
                        // 📐 Match rotation with slot
                        currentGhostInstance.transform.rotation = Quaternion.Euler(-90f, 0f, 0f);
                        
                        // 📏 Fixed scale for ghost mesh
                        currentGhostInstance.transform.localScale = new Vector3(1f, 1.8129f, 1.5688f);
                    }
                    else
                    {
                        currentGhostInstance.transform.position = hoveredSlot.transform.position;
                        currentGhostInstance.transform.rotation = hoveredSlot.transform.rotation;
                    }
                }
            }
            else if (hoveredSlot != null && hoveredSlot.isOccupied)
            {
                // Slot is occupied - show warning color
                if (currentGhostInstance != null)
                {
                    currentGhostInstance.SetActive(true);
                    
                    // Set color to RED (slot is occupied - cannot place)
                    Renderer ghostRenderer = currentGhostInstance.GetComponent<Renderer>();
                    if (ghostRenderer != null)
                    {
                        ghostRenderer.material.color = new Color(1f, 0.2f, 0.2f, 0.5f); // Light red
                    }
                    
                    // Still follow slot position for reference
                    Collider slotCollider = hoveredSlot.GetComponent<Collider>();
                    if (slotCollider != null)
                    {
                        currentGhostInstance.transform.position = slotCollider.bounds.center + new Vector3(0f, -0.2f, 0f);
                        currentGhostInstance.transform.rotation = Quaternion.Euler(-90f, 0f, 0f);
                        currentGhostInstance.transform.localScale = new Vector3(1f, 1.8129f, 1.5688f);
                    }
                }
            }
        }
        else
        {
            // Nếu rê trỏ chuột ra ngoài khu vực ô kệ, ẩn Ghost Mesh đi
            Debug.Log("[UPDATE] Raycast MISS - no slots hit. Ghost mesh hidden.");
            if (currentGhostInstance != null) currentGhostInstance.SetActive(false);
        }
        
        if (hit.collider != null && hit.collider.gameObject.layer == LayerMask.NameToLayer("Slots"))
        {
            // 1. Hiện bảng lên
            if (tooltipPanel != null)
            {
                tooltipPanel.SetActive(true);

                // 2. Cập nhật chữ
                if (titleText != null) titleText.text = "Sách Trinh Thám";
                if (genreText != null) genreText.text = "Thể loại: Crime";

                // 3. Bám theo chuột
                tooltipPanel.transform.position = Input.mousePosition;
            }
        }
        else
        {
            if (tooltipPanel != null) tooltipPanel.SetActive(false);
        }
        // 3. Xử lý cơ chế Thả tay (Drop)
        if (Pointer.current != null && Pointer.current.press.wasReleasedThisFrame)
        {
            ExecuteDrop(hoveredSlot, hit.point);
        }
    }
    public void UpdateTooltip(string title, string genre, Color genreColor)
    {
        titleText.text = title;
        genreText.text = "Thể loại: " + genre;
        genreText.color = genreColor; // Màu chữ thay đổi theo thể loại
    }
    private void ExecuteDrop(WagonSlot slot, Vector3 hitPoint)
    {
        isDragging = false;

        // Phá hủy Ghost Mesh xem trước sau khi thả tay
        if (currentGhostInstance != null) Destroy(currentGhostInstance);

        // Kiểm tra khoảng cách gần để tự động hút (Snapping)
        if (slot != null && !slot.isOccupied && Vector3.Distance(hitPoint, slot.transform.position) <= snapRadius)
        {
            // Trừ 1 cuốn trong Kho chứa (Storage)
            StorageManager.Instance.bookStorage[selectedGenreToPlace]--;

            // Đặt sách vào ô kệ
            slot.PlaceBook(selectedGenreToPlace);

            Debug.Log("[SHELVES] Book placed successfully! Total on shelves: " + StorageManager.Instance.GetTotalBooksOnShelves() + "/64");
        }
        else
        {
            Debug.Log("[KỆ SÁCH] Thả sách THẤT BẠI (Rơi ngoài vùng hoặc ô đã có sách). Hủy thao tác.");
        }

        selectedGenreToPlace = "";
    }
}