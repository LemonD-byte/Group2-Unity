using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Gắn lên DeathPanel. Chỉ chứa hành vi nút Restart / Main Menu khi màn hình chết hiện ra.
/// Việc HIỆN panel này do PlayerHealth.Kill() gọi qua GameStateManager.ShowDeath(),
/// không tự SetActive ở đây.
/// </summary>
public class DeathUI : MonoBehaviour
{
    /// <summary>Gọi từ nút "Chơi lại" (OnClick)</summary>
    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>Gọi từ nút "Về Main Menu" (OnClick)</summary>
    public void BackToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}