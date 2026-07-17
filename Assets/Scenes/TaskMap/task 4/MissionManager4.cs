using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MissionManager4 : MonoBehaviour
{
    public static MissionManager4 Instance { get; private set; }

    [Header("--- CẤU HÌNH YÊU CẦU 3 SAO ---")]
    public float countdownTime = 600f;   // Bộ đếm thời gian 10 phút (600 giây)
    public float minHealthPercent = 50f; // Giữ máu trên 50% để đạt sao 3

    [Header("--- TIẾN ĐỘ ĐỢT 1 (40 ZOMBIE) ---")]
    public int targetWave1Zombies = 40;  // 20 Thường + 10 Runner + 10 Spitter
    private int currentWave1Kills = 0;
    private bool isWave1Complete = false;

    [Header("--- TIẾN ĐỘ ĐỢT 2 (BOSS HULK) ---")]
    private bool isBossSpawned = false;
    private bool isBossKilled = false;

    [Header("--- TEXT HIỂN THỊ HUD REALTIME ---")]
    public TextMeshProUGUI txtHUD_Zombies;   
    public TextMeshProUGUI txtHUD_Timer;     
    public TextMeshProUGUI txtHUD_Health;    
    public TextMeshProUGUI txtHUD_Objectives; 

    [Header("--- THAM CHIẾU BẢNG KẾT QUẢ KHI THẮNG ---")]
    public Slider healthSlider; 
    public GameObject victoryPanel; 
    public TextMeshProUGUI txtResultStar1;
    public TextMeshProUGUI txtResultStar2;
    public TextMeshProUGUI txtResultStar3;

    [Header("--- THAM CHIẾU BỘ SINH BOSS ---")]
    [Tooltip("Kéo GameObject của con Boss Hulk hoặc Spawner chứa Boss vào đây")]
    public GameObject hulkBossObject; 

    private float timeRemaining;
    private bool isLevelEnded = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        currentWave1Kills = 0;
        timeRemaining = countdownTime;
        isWave1Complete = false;
        isBossSpawned = false;
        isBossKilled = false;
        
        if (victoryPanel != null) victoryPanel.SetActive(false);
        
        // Lúc đầu ẩn Boss Hulk đi, diệt hết đợt 1 mới cho xuất hiện
        if (hulkBossObject != null) hulkBossObject.SetActive(false); 

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

    // Hàm nhận mạng từ các script Zombie đợt 1 truyền sang khi chết
    public void RegisterZombieKill()
    {
        if (isLevelEnded) return;

        if (!isWave1Complete)
        {
            currentWave1Kills++;
            if (currentWave1Kills >= targetWave1Zombies)
            {
                currentWave1Kills = targetWave1Zombies;
                isWave1Complete = true;
                SpawnHulkBoss(); // Diệt đủ 40 con đợt 1 -> Boss xuất hiện!
            }
        }
        UpdateHUD();
    }

    // Hàm gọi riêng khi con Boss Hulk bị tiêu diệt
    public void RegisterBossKill()
    {
        if (!isBossSpawned || isLevelEnded) return;
        isBossKilled = true;
        UpdateHUD();
        OnLevelComplete(); 
    }

    void SpawnHulkBoss()
    {
        isBossSpawned = true;
        if (hulkBossObject != null)
        {
            hulkBossObject.SetActive(true);
            Debug.Log("CẢNH BÁO: HULK BOSS ĐÃ XUẤT HIỆN TẠI CĂN CỨ QUÂN SỰ!");
        }
    }

    private void UpdateHUD()
    {
        // 1. Hiển thị số lượng quái / Trạng thái Boss
        if (txtHUD_Zombies != null)
        {
            if (!isWave1Complete)
                txtHUD_Zombies.text = $"Tiến độ đợt 1: {currentWave1Kills}/{targetWave1Zombies}";
            else
                txtHUD_Zombies.text = isBossKilled ? "Boss: Đã tiêu diệt" : "<color=red>Boss: HULK XUẤT HIỆN!</color>";
        }

        // 2. Hiển thị mục tiêu cụ thể
        if (txtHUD_Objectives != null)
        {
            if (!isWave1Complete)
                txtHUD_Objectives.text = "Nhiệm vụ: Tiêu diệt đợt quái tiên phong (40 con)";
            else
                txtHUD_Objectives.text = isBossKilled ? "<color=green>Nhiệm vụ hoàn thành!</color>" : "<color=red>MỤC TIÊU: TIÊU DIỆT BOSS HULK!</color>";
        }

        // 3. Định dạng thời gian Phút:Giây (00:00)
        if (txtHUD_Timer != null)
        {
            int minutes = Mathf.FloorToInt(timeRemaining / 60f);
            int seconds = Mathf.FloorToInt(timeRemaining % 60f);
            txtHUD_Timer.text = string.Format("Thời gian: {0:00}:{1:00}", minutes, seconds);
            if (timeRemaining <= 30f) txtHUD_Timer.color = Color.red;
        }

        // 4. Hiển thị máu
        if (txtHUD_Health != null && healthSlider != null)
        {
            float hpPercent = (healthSlider.value / healthSlider.maxValue) * 100f;
            txtHUD_Health.text = $"Máu: {hpPercent:F0}%";
        }
    }

    // Điều kiện kiểm tra tổng thể
    public bool IsMissionComplete()
    {
        return isWave1Complete && isBossKilled;
    }

    public void OnLevelComplete()
    {
        if (isLevelEnded) return;
        isLevelEnded = true;

        bool isStar1Achieved = isBossKilled; 
        bool isStar2Achieved = (timeRemaining > 0); 

        float currentHealthPercent = 0f;
        if (healthSlider != null)
        {
            currentHealthPercent = (healthSlider.value / healthSlider.maxValue) * 100f;
        }
        bool isStar3Achieved = (currentHealthPercent >= minHealthPercent); 

        ShowEndGameSummary(isStar1Achieved, isStar2Achieved, isStar3Achieved, currentHealthPercent);
    }

    private void ShowEndGameSummary(bool s1, bool s2, bool s3, float finalHealth)
    {
        if (txtHUD_Zombies != null) txtHUD_Zombies.gameObject.SetActive(false);
        if (txtHUD_Timer != null) txtHUD_Timer.gameObject.SetActive(false);
        if (txtHUD_Health != null) txtHUD_Health.gameObject.SetActive(false);
        if (txtHUD_Objectives != null) txtHUD_Objectives.gameObject.SetActive(false);

        if (victoryPanel != null) victoryPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        int minutesSpent = Mathf.FloorToInt((countdownTime - timeRemaining) / 60f);
        int secondsSpent = Mathf.FloorToInt((countdownTime - timeRemaining) % 60f);

        if (txtResultStar1 != null)
            txtResultStar1.text = $"⭐ Tiêu Diệt Boss Hulk: " + (s1 ? "<color=green>HOÀN THÀNH</color>" : "<color=red>THẤT BẠI</color>");

        if (txtResultStar2 != null)
            txtResultStar2.text = string.Format("⭐ Tốc Độ Vượt Ải: {0:00}:{1:00} -> ", minutesSpent, secondsSpent) + (s2 ? "<color=green>ĐẠT</color>" : "<color=red>THẤT BẠI</color>");

        if (txtResultStar3 != null)
            txtResultStar3.text = $"⭐ Máu Ổn Định: {finalHealth:F0}% Máu -> " + (s3 ? "<color=green>ĐẠT</color>" : "<color=red>THẤT BẠI</color>");
    }

    public void RestartLevel() { SceneManager.LoadScene(SceneManager.GetActiveScene().name); }
    public void LoadNextLevel() { SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); }
}