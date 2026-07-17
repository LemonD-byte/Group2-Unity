using TMPro;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [Header("UI")]
    public GameObject inventoryPanel;
    public TMP_Text bombText;
    public TMP_Text healthText;
    public TMP_Text coinText;

    [Header("Inventory")]
    public Inventory inventory;

    private bool isOpen = false;

    void Start()
    {
        // Tự tìm Inventory trong Scene
        if (inventory == null)
        {
            Debug.LogError("Không tìm thấy Inventory trong Scene!");
        }
    }

    void OnEnable()
    {
        // Lắng nghe coin thay đổi để cập nhật ngay cả khi đang mở Inventory
        // (ví dụ nhặt coin ngoài đời thực không xảy ra lúc pause, nhưng vẫn an toàn để có)
        CurrencyManager.OnCoinsChanged += OnCoinsChanged;
    }

    void OnDisable()
    {
        CurrencyManager.OnCoinsChanged -= OnCoinsChanged;
    }

    void OnCoinsChanged(int newAmount)
    {
        if (isOpen && coinText != null)
            coinText.text = newAmount.ToString();
    }

    void Update()
    {
        // Nhấn E để mở / đóng Inventory
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isOpen)
                CloseInventory();
            else
                OpenInventory();
        }

        // Nếu Inventory đang mở thì cập nhật số lượng liên tục
        if (isOpen && inventory != null)
        {
            bombText.text = inventory.grenadeCount.ToString();
            healthText.text = inventory.healPotionCount.ToString();
        }
    }

    public void OpenInventory()
    {
        isOpen = true;

        inventoryPanel.SetActive(true);

        bombText.text = inventory.grenadeCount.ToString();
        healthText.text = inventory.healPotionCount.ToString();
        if (coinText != null) coinText.text = CurrencyManager.Coins.ToString();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0f;
    }

    public void CloseInventory()
    {
        isOpen = false;

        inventoryPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Time.timeScale = 1f;
    }
}