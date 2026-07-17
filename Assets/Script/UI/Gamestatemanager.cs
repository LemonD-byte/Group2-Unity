using UnityEngine;

/// <summary>
/// Bộ điều phối trung tâm cho MỌI UI có thể "chặn" gameplay: Pause, Inventory, Death...
/// Đây là nơi DUY NHẤT được phép đổi Time.timeScale và Cursor trong toàn bộ scene chơi.
/// Mọi script khác (InventoryUI, PauseMenu, PlayerHealth...) không tự ý đổi timeScale/Cursor
/// nữa, mà gọi qua các hàm Open/Close ở đây — nhờ vậy không bao giờ xảy ra chuyện:
///   - Bấm E mở Inventory trong lúc đang Pause -> 2 panel chồng lên nhau.
///   - Đóng Inventory xong Cursor bị khoá nhầm vì Pause vẫn đang mở.
///   - Chết trong lúc đang mở Inventory/Pause -> panel cũ vẫn hiện đè lên màn hình Death.
///
/// Gắn script này vào 1 GameObject rỗng tên "GameManager" trong MỖI scene chơi (TaskMap).
/// </summary>
public class GameStateManager : MonoBehaviour
{
    public enum UIState { Playing, Paused, Inventory, Dead }

    public static GameStateManager Instance { get; private set; }

    [Header("Panel (kéo từ Canvas vào, để sẵn Inactive trong scene)")]
    public GameObject pausePanel;
    public GameObject inventoryPanel;
    public GameObject deathPanel;

    public UIState CurrentState { get; private set; } = UIState.Playing;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            OnEscapePressed();

        if (Input.GetKeyDown(KeyCode.E))
            OnInventoryKeyPressed();
    }

    // ---------- Escape: mở/đóng Pause, hoặc đóng Inventory nếu đang mở ----------
    void OnEscapePressed()
    {
        switch (CurrentState)
        {
            case UIState.Playing:
                OpenPause();
                break;
            case UIState.Paused:
                ClosePause();
                break;
            case UIState.Inventory:
                // Escape ưu tiên đóng Inventory trước, không mở chồng Pause lên trên
                CloseInventory();
                break;
            case UIState.Dead:
                // Đã chết thì Escape không làm gì cả
                break;
        }
    }

    // ---------- E: mở/đóng Inventory, chỉ khi không có panel nào khác đang mở ----------
    void OnInventoryKeyPressed()
    {
        if (CurrentState == UIState.Playing)
            OpenInventory();
        else if (CurrentState == UIState.Inventory)
            CloseInventory();
        // Đang Paused hoặc Dead thì bấm E không có tác dụng gì — tránh mở chồng panel
    }

    // ---------- Pause ----------
    public void OpenPause()
    {
        if (CurrentState != UIState.Playing) return; // không mở chồng lên Inventory/Dead
        CurrentState = UIState.Paused;
        SetPaused(true);
        if (pausePanel != null) pausePanel.SetActive(true);
    }

    public void ClosePause()
    {
        if (CurrentState != UIState.Paused) return;
        CurrentState = UIState.Playing;
        SetPaused(false);
        if (pausePanel != null) pausePanel.SetActive(false);
    }

    // ---------- Inventory ----------
    public void OpenInventory()
    {
        if (CurrentState != UIState.Playing) return;
        CurrentState = UIState.Inventory;
        SetPaused(true);
        if (inventoryPanel != null) inventoryPanel.SetActive(true);
    }

    public void CloseInventory()
    {
        if (CurrentState != UIState.Inventory) return;
        CurrentState = UIState.Playing;
        SetPaused(false);
        if (inventoryPanel != null) inventoryPanel.SetActive(false);
    }

    // ---------- Death: luôn được phép, ghi đè mọi panel khác ----------
    public void ShowDeath()
    {
        CurrentState = UIState.Dead;

        // Đóng hết panel khác đang mở (nếu có) để tránh chồng lấp lên màn hình Death
        if (pausePanel != null) pausePanel.SetActive(false);
        if (inventoryPanel != null) inventoryPanel.SetActive(false);

        SetPaused(true);
        if (deathPanel != null) deathPanel.SetActive(true);
    }

    // ---------- Nơi DUY NHẤT khoá/mở Cursor + timeScale trong toàn bộ scene ----------
    void SetPaused(bool paused)
    {
        Time.timeScale = paused ? 0f : 1f;
        Cursor.lockState = paused ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = paused;
    }
}