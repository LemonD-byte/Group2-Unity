using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Gắn script này vào một GameObject trong scene MainMenu (ví dụ tạo GameObject rỗng tên "ResetManager"),
/// sau đó kéo hàm ResetAllData() (hoặc ResetAllDataWithConfirm() nếu dùng popup xác nhận)
/// vào OnClick() của nút "Reset Game" trên Canvas.
///
/// Danh sách key PlayerPrefs đang được toàn bộ project sử dụng (đã rà soát trong code):
///   - "PlayerCoins"          -> CurrencyManager (tiền)
///   - "Stock_Grenade"        -> InventoryStock (lựu đạn đã mua)
///   - "Stock_HealPotion"     -> InventoryStock (bình hồi máu đã mua)
///   - "SelectedWeapon"       -> WeaponSelectionManager / WeaponManager (vũ khí đang chọn)
///   - "HighestClearedLevel"  -> WeaponSelectionManager (màn cao nhất đã qua -> mở khóa vũ khí)
/// </summary>
public class ResetGameData : MonoBehaviour
{
    [Header("--- (Tuỳ chọn) Popup xác nhận trước khi reset ---")]
    [Tooltip("Panel hỏi 'Bạn có chắc muốn reset?' - kéo vào đây nếu muốn hỏi trước khi xoá.")]
    public GameObject confirmPanel;

    [Tooltip("Text hiển thị thông báo sau khi reset xong (không bắt buộc).")]
    public Text infoText; // đổi thành TMPro.TextMeshProUGUI nếu dự án dùng TMP cho ô này

    // Gọi hàm này từ nút "Reset" để MỞ popup xác nhận (nếu có gán confirmPanel)
    public void OpenConfirmPopup()
    {
        if (confirmPanel != null)
        {
            confirmPanel.SetActive(true);
        }
        else
        {
            // Không có popup thì reset luôn
            ResetAllData();
        }
    }

    // Gọi hàm này từ nút "Huỷ" trong popup xác nhận
    public void CancelReset()
    {
        if (confirmPanel != null) confirmPanel.SetActive(false);
    }

    // Gọi hàm này từ nút "Đồng ý / Xác nhận" trong popup xác nhận
    public void ConfirmReset()
    {
        ResetAllData();
        if (confirmPanel != null) confirmPanel.SetActive(false);
    }

    /// <summary>
    /// Xoá TOÀN BỘ dữ liệu đã lưu của game: vũ khí, tiền tệ, kho đồ, tiến độ màn chơi.
    /// Dùng PlayerPrefs.DeleteAll() để đảm bảo không sót key nào (kể cả key phát sinh sau này).
    /// </summary>
    public void ResetAllData()
    {
        // Xoá sạch mọi key PlayerPrefs của game (coin, vũ khí, kho đồ, màn chơi, v.v...)
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        // Báo cho các hệ thống khác (nếu đang mở sẵn trong scene) cập nhật lại UI ngay lập tức
        CurrencyManager.OnCoinsChanged?.Invoke(0);

        Debug.Log("[ResetGameData] Đã reset toàn bộ dữ liệu game: vũ khí, tiền tệ, kho đồ, màn chơi.");

        if (infoText != null)
            infoText.text = "Đã reset toàn bộ dữ liệu game!";
    }

    /// <summary>
    /// Nếu chỉ muốn xoá đúng các key của game (an toàn hơn DeleteAll trong trường hợp
    /// sau này có thêm PlayerPrefs khác không liên quan đến game này, ví dụ cài đặt âm lượng),
    /// dùng hàm này thay cho ResetAllData().
    /// </summary>
    public void ResetGameKeysOnly()
    {
        PlayerPrefs.DeleteKey("PlayerCoins");        // Reset tiền về 0
        PlayerPrefs.DeleteKey("Stock_Grenade");       // Reset kho lựu đạn
        PlayerPrefs.DeleteKey("Stock_HealPotion");    // Reset kho bình máu
        PlayerPrefs.DeleteKey("SelectedWeapon");      // Reset vũ khí đang chọn
        PlayerPrefs.DeleteKey("HighestClearedLevel"); // Reset tiến độ màn chơi (khoá lại vũ khí)
        PlayerPrefs.Save();

        CurrencyManager.OnCoinsChanged?.Invoke(0);

        Debug.Log("[ResetGameData] Đã reset các key dữ liệu game (giữ nguyên PlayerPrefs khác nếu có).");

        if (infoText != null)
            infoText.text = "Đã reset toàn bộ dữ liệu game!";
    }
}