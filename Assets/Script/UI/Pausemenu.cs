using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Gắn lên PausePanel. Chỉ chứa hành vi của các nút Resume / Restart / Main Menu.
/// Việc MỞ/ĐÓNG panel này (phím Escape) do GameStateManager điều khiển, không tự SetActive
/// hay đổi Time.timeScale ở đây để tránh dẫm chân lên nhau.
/// </summary>
public class PauseMenu : MonoBehaviour
{
    /// <summary>Gọi từ nút "Tiếp tục chơi" (OnClick)</summary>
    public void Resume()
    {
        if (GameStateManager.Instance != null)
            GameStateManager.Instance.ClosePause();
    }

    /// <summary>Gọi từ nút "Chơi lại" (OnClick)</summary>
    public void Restart()
    {
        Time.timeScale = 1f; // trả lại tốc độ bình thường trước khi load lại scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>Gọi từ nút "Về Main Menu" (OnClick)</summary>
    public void BackToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}