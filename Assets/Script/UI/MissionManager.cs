using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using UnityEngine.SceneManagement;
using Enemies;

public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance { get; private set; }

    [Header("--- CẤU HÌNH CHUNG ---")]
    public int targetZombies = 30;
    public float countdownTime = 180f;
    public float minHealthPercent = 50f;

    [Header("--- Task 3: nhặt vật phẩm (để 0 nếu màn không dùng) ---")]
    public int totalMaterialsNeeded = 0;
    private int currentMaterialsCollected = 0;

    [Header("--- Task 4: hoàn thành ngay khi hạ Boss Hulk ---")]
    public bool completeOnBossKill = false;

    [Header("--- HUD ---")]
    public TextMeshProUGUI txtHUD_Zombies;
    public TextMeshProUGUI txtHUD_Timer;
    public TextMeshProUGUI txtHUD_Health;
    public TextMeshProUGUI txtHUD_Objectives;

    [Header("--- BẢNG KẾT QUẢ ---")]
    public Slider healthSlider;
    public GameObject victoryPanel;
    public TextMeshProUGUI txtResultStar1;
    public TextMeshProUGUI txtResultStar2;
    public TextMeshProUGUI txtResultStar3;

    [Header("--- SỰ KIỆN (LevelController lắng nghe để chèn thoại) ---")]
    [Tooltip("Bắn ra NGAY khi đủ điều kiện thắng, TRƯỚC KHI hiện bảng thắng. " +
             "Nếu có LevelController lắng nghe -> nó tự phát thoại kết thúc rồi gọi ShowVictoryPanel(). " +
             "Nếu không ai lắng nghe -> bảng thắng hiện ngay lập tức (giữ hành vi cũ).")]
    public UnityEvent onMissionCompleted;

    [Tooltip("Task 3: bắn ra đúng 1 lần khi vừa đủ vật phẩm, để LevelController phát thoại rồi tự bật spawner đợt 2.")]
    public UnityEvent onMaterialsCompleted;

    [Header("--- UNLOCK VŨ KHÍ (bắt buộc phải điền cho từng màn) ---")]
    public int levelIndex = 1; // Level1 -> 1, Level2 -> 2 ...

    private int currentKills = 0;
    private float timeRemaining;
    private bool isLevelEnded = false;

    private bool s1Cached, s2Cached, s3Cached;
    private float finalHealthCached;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        EnemyBase.OnAnyZombieDied += HandleZombieDied;
    }

    void OnDestroy()
    {
        EnemyBase.OnAnyZombieDied -= HandleZombieDied;
    }

    void Start()
    {
        currentKills = 0;
        currentMaterialsCollected = 0;
        timeRemaining = countdownTime;
        if (victoryPanel != null) victoryPanel.SetActive(false);
        UpdateHUD();
    }

    void Update()
    {
        if (isLevelEnded) return;

        // Đóng băng thời gian khi đang hiện hộp thoại — chỉ tính giờ sau khi thoại đóng lại
        if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive)
        {
            UpdateHUD();
            return;
        }

        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            if (timeRemaining < 0) timeRemaining = 0;
        }
        UpdateHUD();
    }

    private void HandleZombieDied(EnemyBase enemy)
    {
        if (isLevelEnded) return;

        currentKills++;

        // Task 4: con vừa chết CHÍNH LÀ Boss Hulk -> thắng ngay
        if (completeOnBossKill && enemy is HulkZombie)
        {
            OnLevelComplete();
            return;
        }

        UpdateHUD();
    }

    // Task 3: gọi từ ObjectiveItem.cs khi nhặt 1 vật phẩm
    public void CollectMaterial()
    {
        if (isLevelEnded) return;
        currentMaterialsCollected++;
        UpdateHUD();

        if (currentMaterialsCollected == totalMaterialsNeeded)
            onMaterialsCompleted?.Invoke();
    }

    private void UpdateHUD()
    {
        if (txtHUD_Zombies != null)
            txtHUD_Zombies.text = totalMaterialsNeeded > 0
                ? $"Đã diệt: {currentKills} Zombie"
                : $"Zombie: {currentKills}/{targetZombies}";

        if (txtHUD_Timer != null)
            txtHUD_Timer.text = $"Thời gian: {timeRemaining:F0}s";

        if (txtHUD_Health != null && healthSlider != null)
        {
            float hpPercent = (healthSlider.value / healthSlider.maxValue) * 100f;
            txtHUD_Health.text = $"Máu: {hpPercent:F0}%";
        }

        if (txtHUD_Objectives != null)
        {
            if (totalMaterialsNeeded > 0)
                txtHUD_Objectives.text = currentMaterialsCollected < totalMaterialsNeeded
                    ? $"Nhiệm vụ: Tìm nguyên liệu thuốc ({currentMaterialsCollected}/{totalMaterialsNeeded})"
                    : "<color=green>ĐỦ NGUYÊN LIỆU! CHẠY NGAY RA XE!</color>";
            else if (completeOnBossKill)
                txtHUD_Objectives.text = "Nhiệm vụ: Sống sót & tiêu diệt Boss Hulk";
            else
                txtHUD_Objectives.text = $"Nhiệm vụ: Tiêu diệt {targetZombies} Zombie!";
        }
    }

    public bool IsMissionComplete()
    {
        if (totalMaterialsNeeded > 0) return currentMaterialsCollected >= totalMaterialsNeeded;
        if (completeOnBossKill) return isLevelEnded;
        return currentKills >= targetZombies;
    }

    public void OnLevelComplete()
    {
        if (isLevelEnded) return;
        isLevelEnded = true;

        s1Cached = totalMaterialsNeeded > 0
            ? currentMaterialsCollected >= totalMaterialsNeeded
            : (completeOnBossKill ? true : currentKills >= targetZombies);
        s2Cached = (timeRemaining > 0);

        finalHealthCached = 0f;
        if (healthSlider != null)
            finalHealthCached = (healthSlider.value / healthSlider.maxValue) * 100f;
        s3Cached = (finalHealthCached >= minHealthPercent);

        if (s1Cached) // chỉ mở khóa khi thật sự đạt mục tiêu, không tính hết giờ/thua
        {
            WeaponSelectionManager.UnlockUpToLevel(levelIndex);
        }

        if (txtHUD_Zombies != null) txtHUD_Zombies.gameObject.SetActive(false);
        if (txtHUD_Timer != null) txtHUD_Timer.gameObject.SetActive(false);
        if (txtHUD_Health != null) txtHUD_Health.gameObject.SetActive(false);
        if (txtHUD_Objectives != null) txtHUD_Objectives.gameObject.SetActive(false);

        if (onMissionCompleted != null && onMissionCompleted.GetPersistentEventCount() > 0)
            onMissionCompleted.Invoke(); // LevelController sẽ tự gọi ShowVictoryPanel() sau khi thoại xong
        else
            ShowVictoryPanel(); 
    }

    public void ShowVictoryPanel()
    {
        if (victoryPanel != null) victoryPanel.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (txtResultStar1 != null)
            txtResultStar1.text = (s1Cached ? "<color=green>ĐẠT</color>" : "<color=red>THẤT BẠI</color>");
        if (txtResultStar2 != null)
            txtResultStar2.text = $"⭐ Tốc Độ: Còn {timeRemaining:F1}s dư -> " + (s2Cached ? "<color=green>ĐẠT</color>" : "<color=red>THẤT BẠI</color>");
        if (txtResultStar3 != null)
            txtResultStar3.text = $"⭐ Giữ Máu: {finalHealthCached:F0}% / {minHealthPercent}% -> " + (s3Cached ? "<color=green>ĐẠT</color>" : "<color=red>THẤT BẠI</color>");
    }

    public void RestartLevel() { SceneManager.LoadScene(SceneManager.GetActiveScene().name); }
    public void LoadNextLevel() { SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); }
}