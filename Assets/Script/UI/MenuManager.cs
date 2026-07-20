using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Gắn script này vào GameObject "Canvas" (hoặc 1 GameObject quản lý riêng) trong scene MainMenu.
/// Kéo 4 panel tương ứng vào 4 ô Inspector bên dưới.
///
/// KHÓA MÀN THEO TRÌNH TỰ:
/// - Dùng lại đúng PlayerPrefs "HighestClearedLevel" mà WeaponSelectionManager/MissionManager
///   đã dùng để mở khóa vũ khí, nên không cần sửa gì ở MissionManager.
/// - Trong mảng "Map Buttons" ở Inspector, mỗi màn cần khai báo:
///     sceneName            = đúng tên scene (vd: "task 1", "task 2"...)
///     requiredClearedLevel = số màn tối thiểu phải qua trước đó
///         task 1 -> 0 (luôn mở sẵn)
///         task 2 -> 1
///         task 3 -> 2
///         task 4 -> 3
///     button    = kéo Button của map đó vào
///     lockIcon  = (tùy chọn) icon ổ khóa hiện lên khi bị khóa
/// </summary>
public class MenuManager : MonoBehaviour
{
    [System.Serializable]
    public class MapButtonInfo
    {
        public string sceneName;
        public int requiredClearedLevel;
        public Button button;
        public GameObject lockIcon; // tuỳ chọn, có thể để trống
    }

    [Header("Các Panel")]
    public GameObject mainPanel;
    public GameObject mapSelectPanel;
    public GameObject shopPanel;

    [Header("--- KHÓA MÀN THEO TRÌNH TỰ ---")]
    public MapButtonInfo[] mapButtons;
    public Color lockedColor = new Color(0.3f, 0.3f, 0.3f, 1f);
    public Color unlockedColor = Color.white;

    void Start()
    {
        ShowMain();
        RefreshMapLockState();
    }

    // ----- Điều hướng giữa các Panel -----

    public void ShowMain()
    {
        SetActivePanel(mainPanel);
    }

    public void ShowMapSelect()
    {
        SetActivePanel(mapSelectPanel);
        RefreshMapLockState(); // cập nhật lại mỗi lần mở panel chọn màn, tránh dùng dữ liệu cũ
    }

    public void ShowShop()
    {
        SetActivePanel(shopPanel);
    }

    private void SetActivePanel(GameObject panelToShow)
    {
        mainPanel.SetActive(panelToShow == mainPanel);
        mapSelectPanel.SetActive(panelToShow == mapSelectPanel);
        shopPanel.SetActive(panelToShow == shopPanel);
    }

    // ----- Khóa/mở màn -----

    /// <summary>Gọi lại hàm này mỗi khi vào MapSelectPanel để cập nhật đúng trạng thái khóa.</summary>
    public void RefreshMapLockState()
    {
        int highestCleared = PlayerPrefs.GetInt("HighestClearedLevel", 0);

        foreach (var m in mapButtons)
        {
            if (m.button == null) continue;

            bool unlocked = highestCleared >= m.requiredClearedLevel;

            m.button.interactable = unlocked;

            if (m.button.image != null)
                m.button.image.color = unlocked ? unlockedColor : lockedColor;

            if (m.lockIcon != null)
                m.lockIcon.SetActive(!unlocked);

            // Gán lại onClick mỗi lần refresh để tránh add nhiều listener trùng
            string scene = m.sceneName;
            m.button.onClick.RemoveAllListeners();
            if (unlocked)
                m.button.onClick.AddListener(() => LoadMap(scene));
        }
    }

    // ----- Chọn map và vào scene -----

    /// <summary>Gọi hàm này từ OnClick() của từng nút map, truyền đúng tên Scene.</summary>
    public void LoadMap(string sceneName)
    {
        // Chặn phòng hờ: kể cả khi có ai đó gọi LoadMap() bằng cách khác (không qua nút bấm),
        // vẫn không cho vào màn chưa unlock -> tránh việc lách khóa màn.
        var info = System.Array.Find(mapButtons, m => m.sceneName == sceneName);
        if (info != null)
        {
            int highestCleared = PlayerPrefs.GetInt("HighestClearedLevel", 0);
            if (highestCleared < info.requiredClearedLevel)
            {
                Debug.LogWarning($"[MenuManager] Màn '{sceneName}' đang bị khóa, chưa thể vào.");
                return;
            }
        }

        SceneManager.LoadScene(sceneName);
    }

    // ----- Thoát game (tuỳ chọn, nếu muốn thêm nút Quit) -----
    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}