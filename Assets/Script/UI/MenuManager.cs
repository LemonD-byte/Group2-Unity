using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Gắn script này vào GameObject "Canvas" (hoặc 1 GameObject quản lý riêng) trong scene MainMenu.
/// Kéo 4 panel tương ứng vào 4 ô Inspector bên dưới.
/// </summary>
public class MenuManager : MonoBehaviour
{
    [Header("Các Panel")]
    public GameObject mainPanel;
    public GameObject mapSelectPanel;
    public GameObject shopPanel;
    void Start()
    {
        ShowMain();
    }

    // ----- Điều hướng giữa các Panel -----

    public void ShowMain()
    {
        SetActivePanel(mainPanel);
    }

    public void ShowMapSelect()
    {
        SetActivePanel(mapSelectPanel);
    }

    public void ShowShop()
    {
        SetActivePanel(shopPanel); // hiện tại để rỗng, sau này làm thêm nội dung
    }

    private void SetActivePanel(GameObject panelToShow)
    {
        mainPanel.SetActive(panelToShow == mainPanel);
        mapSelectPanel.SetActive(panelToShow == mapSelectPanel);
        shopPanel.SetActive(panelToShow == shopPanel);
    }

    // ----- Chọn map và vào scene -----

    /// <summary>Gọi hàm này từ OnClick() của từng nút map, truyền đúng tên Scene.</summary>
    public void LoadMap(string sceneName)
    {
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