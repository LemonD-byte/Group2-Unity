using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// Bộ điều phối trung tâm cho MỌI UI và lưu trữ dữ liệu vũ khí toàn cục.
/// </summary>
public class GameStateManager : MonoBehaviour
{
    public enum UIState { Playing, Paused, Inventory, Dead }

    // Đổi sang kiểu public static chuẩn để gọi GameManager.Instance từ mọi nơi
    public static GameStateManager Instance { get; private set; }

    [Header("Panel (kéo từ Canvas vào, để sẵn Inactive trong scene)")]
    public GameObject pausePanel;
    public GameObject inventoryPanel;
    public GameObject deathPanel;

    // ==========================================
    // KHU VỰC MẠNH THÊM VÀO: LƯU TRỮ DỮ LIỆU CHỌN VŨ KHÍ
    // ==========================================
    [Header("Dữ liệu chọn màn & Vũ khí (Mạnh thêm)")]
    public int currentMapIndex = 1; 
    public string selectedMainWeapon = "";
    public string selectedSecondaryWeapon = "pistol"; // Mặc định có pistol
    public string selectedMeleeWeapon = "baseball_bat";

    [Header("Shop Upgrades (Mạnh thêm)")]
    public float damageMultiplier = 1f; // Thuốc tăng sát thương từ Shop
    // ==========================================

    public UIState CurrentState { get; private set; } = UIState.Playing;

    void Awake()
    {
        // Giữ lại cơ chế Singleton nhưng thêm DontDestroyOnLoad để giữ dữ liệu khi đổi Map
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        // Chỉ chạy các phím tắt UI nếu đang ở trong màn chơi chính (Tránh bấm nhầm ngoài Main Menu)
        if (SceneManager.GetActiveScene().name != "MainMenu") 
        {
            if (Input.GetKeyDown(KeyCode.Escape))
                OnEscapePressed();

            if (Input.GetKeyDown(KeyCode.E))
                OnInventoryKeyPressed();
        }
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
                CloseInventory();
                break;
            case UIState.Dead:
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
    }

    // ---------- Pause ----------
    public void OpenPause()
    {
        if (CurrentState != UIState.Playing) return; 
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