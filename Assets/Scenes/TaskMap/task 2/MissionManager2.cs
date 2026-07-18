using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MissionManager2 : MonoBehaviour
{
    public static MissionManager2 Instance { get; private set; }

    [Header("--- CẤU HÌNH YÊU CẦU MÀN CHƠI ---")]
    public int targetZombies = 25; // 20 Thường + 5 Spritter
    public float countdownTime = 180f; 
    public float minHealthPercent = 50f;

    [Header("--- TEXT HIỂN THỊ KHI ĐANG CHƠI (HUD) ---")]
    public TextMeshProUGUI txtHUD_Zombies; 
    public TextMeshProUGUI txtHUD_Timer;   
    public TextMeshProUGUI txtHUD_Health; 
    public TextMeshProUGUI txtHUD_Objectives; // Text hiển thị trạng thái tìm Gia đình

    [Header("--- THAM CHIẾU BẢNG KẾT QUẢ KHI THẮNG ---")]
    public Slider healthSlider; 
    public GameObject victoryPanel; 
    public TextMeshProUGUI txtResultStar1;
    public TextMeshProUGUI txtResultStar2;
    public TextMeshProUGUI txtResultStar3;

    private int currentKills = 0;
    private float timeRemaining;
    private bool isLevelEnded = false;

    // Trạng thái giải cứu gia đình 
    private bool isFamilyRescued = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        currentKills = 0;
        timeRemaining = countdownTime;
        isFamilyRescued = false;
        
        if (victoryPanel != null) victoryPanel.SetActive(false);
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

    public void RegisterMultipleZombiesKilled(int amount)
    {
        if (isLevelEnded) return;
        currentKills += amount; 
        if (currentKills > targetZombies) currentKills = targetZombies;
        UpdateHUD();           
    }

    // Hàm gọi khi người chơi tương tác thành công với vị trí của 2 mẹ con
    public void RescueFamily()
    {
        isFamilyRescued = true;
        UpdateHUD();
    }

    private void UpdateHUD()
    {
        if (txtHUD_Zombies != null)
            txtHUD_Zombies.text = $"Zombie: {currentKills}/{targetZombies}";

        if (txtHUD_Timer != null)
            txtHUD_Timer.text = $"Thời gian: {timeRemaining:F0}s";

        if (txtHUD_Health != null && healthSlider != null)
        {
            float hpPercent = (healthSlider.value / healthSlider.maxValue) * 100f;
            txtHUD_Health.text = $"Máu: {hpPercent:F0}%";
        }

        // Giao diện HUD hiển thị ngắn gọn, trực quan
        if (txtHUD_Objectives != null)
        {
            txtHUD_Objectives.text = isFamilyRescued 
                ? "Nhiệm vụ: <color=green>[Gia đình: Đã an toàn]</color> -> Chạy ra xe" 
                : "Nhiệm vụ: <color=red>[Tìm kiếm Vợ & Con]</color>";
        }
    }

    public void OnLevelComplete()
    {
        if (isLevelEnded) return;
        isLevelEnded = true;

        // Sao 1: Diệt hết quái + Đã tìm thấy gia đình
        bool isStar1Achieved = (currentKills >= targetZombies) && isFamilyRescued; 
        // Sao 2: Qua màn nhanh (Thời gian còn lại > 0)
        bool isStar2Achieved = (timeRemaining > 0);            

        float currentHealthPercent = 0f;
        if (healthSlider != null)
        {
            currentHealthPercent = (healthSlider.value / healthSlider.maxValue) * 100f;
        }
        // Sao 3: Giữ máu an toàn
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
        float timeSpent = countdownTime - timeRemaining;

        if (txtResultStar1 != null)
            txtResultStar1.text = $"⭐ Đoàn Tụ Gia Đình: " + (s1 ? "<color=green>HOÀN THÀNH</color>" : "<color=red>THẤT BẠI</color>");

        if (txtResultStar2 != null)
            txtResultStar2.text = $"⭐ Tốc Độ Vượt Ải: Dùng {timeSpent:F1}s -> " + (s2 ? "<color=green>ĐẠT</color>" : "<color=red>THẤT BẠI</color>");

        if (txtResultStar3 != null)
            txtResultStar3.text = $"⭐ Máu: {finalHealth:F0}% Máu -> " + (s3 ? "<color=green>ĐẠT</color>" : "<color=red>THẤT BẠI</color>");
    }
    public void RestartLevel()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    public void LoadNextLevel()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex + 1);
    }
    public bool IsMissionComplete()
    {
        return currentKills >= targetZombies && isFamilyRescued;
    }
}