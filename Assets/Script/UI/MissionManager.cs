using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance { get; private set; }

    [Header("--- CẤU HÌNH YÊU CẦU 3 SAO ---")]
    public int targetZombies = 30;
    public float countdownTime = 180f; // Đếm ngược từ 180 giây
    public float minHealthPercent = 50f;

    [Header("--- TEXT HIỂN THỊ KHI ĐANG CHƠI (HUD) ---")]
    public TextMeshProUGUI txtHUD_Zombies; 
    public TextMeshProUGUI txtHUD_Timer;   
    public TextMeshProUGUI txtHUD_Health; 

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

        // Xử lý đếm ngược
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            if (timeRemaining < 0) timeRemaining = 0;
        }

        UpdateHUD();
    }

    public void RegisterZombieKill()
    {
        if (isLevelEnded) return;
        currentKills++;
        UpdateHUD();
    }

    private void UpdateHUD()
    {
        // 1. Cập nhật số zombie diệt được
        if (txtHUD_Zombies != null)
            txtHUD_Zombies.text = $"Zombie: {currentKills}/{targetZombies}";

        // 2. Cập nhật thời gian đếm ngược
        if (txtHUD_Timer != null)
        {
            txtHUD_Timer.text = $"Thời gian: {timeRemaining:F0}s";
            if (timeRemaining <= 10f) txtHUD_Timer.color = Color.red;
        }

        // 3. Cập nhật % máu realtime lên HUD chữ từ Slider của bạn
        if (txtHUD_Health != null && healthSlider != null)
        {
            float hpPercent = (healthSlider.value / healthSlider.maxValue) * 100f;
            txtHUD_Health.text = $"Máu: {hpPercent:F0}%";
        }
    }

    public void OnLevelComplete()
    {
        if (isLevelEnded) return;
        isLevelEnded = true;

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
        // Ẩn toàn bộ HUD chơi game
        if (txtHUD_Zombies != null) txtHUD_Zombies.gameObject.SetActive(false);
        if (txtHUD_Timer != null) txtHUD_Timer.gameObject.SetActive(false);
        if (txtHUD_Health != null) txtHUD_Health.gameObject.SetActive(false);

        // Hiện bảng kết quả kết thúc màn
        if (victoryPanel != null) victoryPanel.SetActive(true);

        float timeSpent = countdownTime - timeRemaining;

        if (txtResultStar1 != null)
            txtResultStar1.text = $"⭐ Diệt Zombie: {currentKills}/{targetZombies} -> " + (s1 ? "<color=green>ĐẠT</color>" : "<color=red>THẤT BẠI</color>");

        if (txtResultStar2 != null)
            txtResultStar2.text = $"⭐ Thời gian: Dùng {timeSpent:F1}s (Còn dư {timeRemaining:F1}s) -> " + (s2 ? "<color=green>ĐẠT</color>" : "<color=red>THẤT BẠI</color>");

        if (txtResultStar3 != null)
            txtResultStar3.text = $"⭐ Máu còn lại: {finalHealth:F0}% / {minHealthPercent}% -> " + (s3 ? "<color=green>ĐẠT</color>" : "<color=red>THẤT BẠI</color>");
    }
}