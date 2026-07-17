using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance { get; private set; }

    [Header("--- CẤU HÌNH YÊU CẦU MÀN CHƠI ---")]
    public int targetZombies = 30; 
    public float countdownTime = 180f; 
    public float minHealthPercent = 50f; 

    [Header("--- TEXT HIỂN THỊ KHI ĐANG CHƠI (HUD) ---")]
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

    private int currentKills = 0;
    private float timeRemaining;
    private bool isLevelEnded = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        currentKills = 0;
        timeRemaining = countdownTime;
        
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

    // Hàm nhận tín hiệu mỗi khi một con zombie bị hạ gục
    public void RegisterMultipleZombiesKilled(int amount)
    {
        if (isLevelEnded) return;
        currentKills += amount; 
        if (currentKills > targetZombies) currentKills = targetZombies;
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

        if (txtHUD_Objectives != null)
        {
            txtHUD_Objectives.text = "Nhiệm vụ: Tiêu diệt toàn bộ 30 Zombie!";
        }
    }

    // Kiểm tra điều kiện khi người chơi chạy ra xe trốn thoát
    public bool IsMissionComplete()
    {
        return currentKills >= targetZombies;
    }

    // Hàm kích hoạt bảng chiến thắng khi qua màn
    public void OnLevelComplete()
    {
        if (isLevelEnded) return;
        isLevelEnded = true;

        // Tính toán 3 mốc Sao
        bool isStar1Achieved = (currentKills >= targetZombies); 
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
        // Ẩn HUD chơi game
        if (txtHUD_Zombies != null) txtHUD_Zombies.gameObject.SetActive(false);
        if (txtHUD_Timer != null) txtHUD_Timer.gameObject.SetActive(false);
        if (txtHUD_Health != null) txtHUD_Health.gameObject.SetActive(false);
        if (txtHUD_Objectives != null) txtHUD_Objectives.gameObject.SetActive(false);

        // Hiện bảng Victory
        if (victoryPanel != null) victoryPanel.SetActive(true);
        
        // Hiện chuột để bấm nút chuyển màn
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        float timeSpent = countdownTime - timeRemaining;

        if (txtResultStar1 != null)
            txtResultStar1.text = $"⭐ Càn Quét Zombie: {currentKills}/{targetZombies} -> " + (s1 ? "<color=green>ĐẠT</color>" : "<color=red>THẤT BẠI</color>");

        if (txtResultStar2 != null)
            txtResultStar2.text = $"⭐ Tốc Độ Sinh Tồn: Còn {timeRemaining:F1}s dư -> " + (s2 ? "<color=green>ĐẠT</color>" : "<color=red>THẤT BẠI</color>");

        if (txtResultStar3 != null)
            txtResultStar3.text = $"⭐ Giữ Máu An Toàn: {finalHealth:F0}% / {minHealthPercent}% -> " + (s3 ? "<color=green>ĐẠT</color>" : "<color=red>THẤT BẠI</color>");
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadNextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}