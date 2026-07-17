using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MissionManager3 : MonoBehaviour
{
    public static MissionManager3 Instance { get; private set; }

    [Header("--- CẤU HÌNH YÊU CẦU 3 SAO ---")]
    public float countdownTime = 240f; // 4 phút cho màn chơi bệnh viện
    public float minHealthPercent = 50f;

    [Header("--- TIẾN ĐỘ NHIỆM VỤ MÀN 3 ---")]
    public int totalMaterialsNeeded = 3;
    private int currentMaterialsCollected = 0;
    
    private int currentKills = 0; // Vẫn đếm số zombie diệt được để hiển thị cho đẹp

    [Header("--- TEXT HIỂN THỊ HUD REALTIME ---")]
    public TextMeshProUGUI txtHUD_Zombies; // Hiện: "Đã diệt: X" (cho người chơi theo dõi)
    public TextMeshProUGUI txtHUD_Timer;   
    public TextMeshProUGUI txtHUD_Health; 
    public TextMeshProUGUI txtHUD_Objectives; // Hiện tiến độ: "Nhiệm vụ: Tìm nguyên liệu thuốc (X/3)"

    [Header("--- THAM CHIẾU BẢNG KẾT QUẢ KHI THẮNG ---")]
    public Slider healthSlider; 
    public GameObject victoryPanel; 
    public TextMeshProUGUI txtResultStar1;
    public TextMeshProUGUI txtResultStar2;
    public TextMeshProUGUI txtResultStar3;

    [Header("--- THAM CHIẾU BỘ SINH QUÁI (SPAWNER) ---")]
    [Tooltip("Kéo ZombieSpawner của đợt 2 vào đây để tự kích hoạt khi nhặt đủ đồ")]
    public GameObject zombieWave2Spawner; 

    private float timeRemaining;
    private bool isLevelEnded = false;
    private bool isWave2Triggered = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        currentKills = 0;
        currentMaterialsCollected = 0;
        timeRemaining = countdownTime;
        
        if (victoryPanel != null) victoryPanel.SetActive(false);
        if (zombieWave2Spawner != null) zombieWave2Spawner.SetActive(false); // Đợt 2 ẩn lúc đầu

        UpdateHUD();
    }

    void Update()
    {
        if (isLevelEnded) return;

        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            if (timeRemaining < 0) timeRemaining = 0;
        }

        UpdateHUD();
    }

    // Hàm xử lý khi nhặt được 1 nguyên liệu thuốc giải
    public void CollectMaterial()
    {
        if (isLevelEnded) return;

        currentMaterialsCollected++;
        UpdateHUD();

        // Thu thập đủ 3 nguyên liệu -> Kích hoạt Đợt 2 (Zombie tràn ra cản đường cực kỳ dồn dập)
        if (currentMaterialsCollected >= totalMaterialsNeeded && !isWave2Triggered)
        {
            TriggerWave2();
        }
    }

    void TriggerWave2()
    {
        isWave2Triggered = true;
        
        // Bật bộ sinh quái đợt 2 tràn vào làm tăng độ kịch tính khi chạy trốn
        if (zombieWave2Spawner != null)
        {
            zombieWave2Spawner.SetActive(true);
            Debug.Log("CẢNH BÁO: Đợt Zombie thứ 2 xuất hiện!");
        }
    }

    // Các hàm này vẫn giữ để khi người chơi bắn/ném bom zombie thì HUD vẫn cộng điểm diệt quái cho sinh động
    public void RegisterZombieKill()
    {
        if (isLevelEnded) return;
        currentKills++;
        UpdateHUD();
    }

    public void RegisterMultipleZombiesKilled(int amount)
    {
        if (isLevelEnded) return;
        currentKills += amount;
        UpdateHUD();
    }

    private void UpdateHUD()
    {
        // 1. Cập nhật số lượng Zombie đã diệt
        if (txtHUD_Zombies != null)
            txtHUD_Zombies.text = $"Đã diệt: {currentKills} Zombie";

        // 2. Cập nhật Tiến độ tìm Nguyên liệu thuốc giải
        if (txtHUD_Objectives != null)
        {
            if (currentMaterialsCollected < totalMaterialsNeeded)
                txtHUD_Objectives.text = $"Nhiệm vụ: Tìm nguyên liệu thuốc ({currentMaterialsCollected}/{totalMaterialsNeeded})";
            else
                txtHUD_Objectives.text = $"<color=green>ĐỦ NGUYÊN LIỆU! CHẠY NGAY RA XE!</color>";
        }

        // 3. Cập nhật thời gian
        if (txtHUD_Timer != null)
        {
            txtHUD_Timer.text = $"Thời gian: {timeRemaining:F0}s";
            if (timeRemaining <= 15f) txtHUD_Timer.color = Color.red;
        }

        // 4. Cập nhật máu
        if (txtHUD_Health != null && healthSlider != null)
        {
            float hpPercent = (healthSlider.value / healthSlider.maxValue) * 100f;
            txtHUD_Health.text = $"Máu: {hpPercent:F0}%";
        }
    }

    // 🌟 ĐIỀU KIỆN QUA MÀN: Chỉ cần nhặt đủ 3 nguyên liệu là xe mở khóa (Không cần giết sạch Zombie)
    public bool IsMissionComplete()
    {
        return (currentMaterialsCollected >= totalMaterialsNeeded);
    }

    public void OnLevelComplete()
    {
        if (isLevelEnded) return;
        isLevelEnded = true;

        // Tính toán tiêu chí 3 Sao
        bool isStar1Achieved = (currentMaterialsCollected >= totalMaterialsNeeded); // Sao 1: Nhặt đủ 3 thuốc
        bool isStar2Achieved = (timeRemaining > 0);                                  // Sao 2: Còn thời gian

        float currentHealthPercent = 0f;
        if (healthSlider != null)
        {
            currentHealthPercent = (healthSlider.value / healthSlider.maxValue) * 100f;
        }
        bool isStar3Achieved = (currentHealthPercent >= minHealthPercent);           // Sao 3: Máu trên 50%

        ShowEndGameSummary(isStar1Achieved, isStar2Achieved, isStar3Achieved, currentHealthPercent);
    }

    private void ShowEndGameSummary(bool s1, bool s2, bool s3, float finalHealth)
    {
        if (txtHUD_Zombies != null) txtHUD_Zombies.gameObject.SetActive(false);
        if (txtHUD_Timer != null) txtHUD_Timer.gameObject.SetActive(false);
        if (txtHUD_Health != null) txtHUD_Health.gameObject.SetActive(false);
        if (txtHUD_Objectives != null) txtHUD_Objectives.gameObject.SetActive(false);

        if (victoryPanel != null) victoryPanel.SetActive(true);

        float timeSpent = countdownTime - timeRemaining;

        if (txtResultStar1 != null)
            txtResultStar1.text = $"⭐ Đủ nguyên liệu thuốc: " + (s1 ? "<color=green>ĐẠT</color>" : "<color=red>THẤT BẠI</color>");

        if (txtResultStar2 != null)
            txtResultStar2.text = $"⭐ Tốc độ thoát thân: Dùng {timeSpent:F1}s (Còn dư {timeRemaining:F1}s) -> " + (s2 ? "<color=green>ĐẠT</color>" : "<color=red>THẤT BẠI</color>");

        if (txtResultStar3 != null)
            txtResultStar3.text = $"⭐ Máu an toàn: {finalHealth:F0}% / {minHealthPercent}% -> " + (s3 ? "<color=green>ĐẠT</color>" : "<color=red>THẤT BẠI</color>");
    }
}