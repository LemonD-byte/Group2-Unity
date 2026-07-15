using UnityEngine;
using UnityEngine.UI; // BẮT BUỘC: để điều khiển Slider
using TMPro;         // BẮT BUỘC: để điều khiển TextMeshPro

public class PlayerHealth : MonoBehaviour
{
    [Header("Player Stats")]
    public float maxHealth = 100f;

    [Header("Hồi máu bằng phím tắt")]
    [Tooltip("Lượng máu hồi mỗi lần bấm phím / dùng 1 bình")]
    public float healAmountPerUse = 30f;
    [Tooltip("Phím dùng để hồi máu")]
    public KeyCode healKey = KeyCode.H;
    public float healCooldown = 1f;

    [Header("Inventory")]
    [Tooltip("Kéo Inventory của Player vào đây. Để trống thì tự tìm trên cùng GameObject")]
    public Inventory inventory;

    [Header("Giao diện UI Máu")]
    public Slider healthSlider;      // Kéo ô HealthSlider ngoài Canvas vào đây
    public TextMeshProUGUI hpText;   // Kéo ô HPText ngoài Canvas vào đây

    private float nextHealTime;
    private float currentHealth;
    private bool isDead;

    public float CurrentHealth => currentHealth;
    public bool IsDead => isDead;

    [Tooltip("Số bình hồi máu còn lại, đọc từ Inventory - dùng để hiển thị UI nếu cần")]
    public int HealPotionCount => inventory != null ? inventory.healPotionCount : 0;

    void Awake()
    {
        if (inventory == null)
            inventory = GetComponent<Inventory>();
    }

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI(); // Mới vào trận thì đầy máu, hiện luôn UI đúng ngay từ đầu
    }

    void Update()
    {
        if (Input.GetKeyDown(healKey))
        {
            TryHeal();
        }
    }

    /// <summary>
    /// Gọi hàm này từ phím tắt (mặc định H) hoặc từ Button UI (OnClick) để dùng
    /// 1 bình hồi máu từ Inventory, hồi "healAmountPerUse" máu, có tính healCooldown.
    /// </summary>
    public bool TryHeal()
    {
        if (isDead) return false;
        if (Time.time < nextHealTime) return false;

        // Hết bình hồi máu trong Inventory thì không hồi được
        if (inventory != null && !inventory.UseHealPotion()) return false;

        Heal(healAmountPerUse);
        nextHealTime = Time.time + healCooldown;
        return true;
    }

    public void Heal(float amount)
    {
        if (isDead || amount <= 0f) return;

        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);

        UpdateHealthUI(); // Hồi máu xong thì thanh UI tự dài ra lại
        Debug.Log("Player hồi " + amount + " máu. Máu hiện tại: " + currentHealth);
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        UpdateHealthUI(); // Mỗi lần mất máu thì thanh UI tự co lại
        Debug.Log("Player nhận " + damage + " sát thương");
        Debug.Log("Máu còn: " + currentHealth);

        if (currentHealth <= 0)
        {
            Kill();
        }
    }

    // --- HÀM ĐIỀU KHIỂN THANH TRƯỢT + CHỮ SỐ MÁU ---
    void UpdateHealthUI()
    {
        // Ép thanh trượt Slider co/dãn theo số máu hiện tại
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

        // Ép chữ số thay đổi theo (Ví dụ: 80 / 100)
        if (hpText != null)
        {
            hpText.text = Mathf.CeilToInt(currentHealth) + " / " + Mathf.CeilToInt(maxHealth);
        }
    }

    public void Kill()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("Player đã chết");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}