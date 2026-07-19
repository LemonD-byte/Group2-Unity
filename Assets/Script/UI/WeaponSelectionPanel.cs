using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class WeaponSelectionPanel : MonoBehaviour
{
    [System.Serializable]
    public class WeaponButton
    {
        public string weaponID;             // Mã vũ khí (Ví dụ: "ak_47", "shotgun", "melee_bat")
        public Button buttonComponent;      // Nút bấm của vũ khí đó
        public TextMeshProUGUI buttonText;  // Chữ hiển thị tên vũ khí để làm mờ
        public int requiredMap = 1;         // Map tối thiểu để được xài khẩu này
    }

    [Header("Danh sách nút vũ khí")]
    public WeaponButton[] allWeapons;

    void OnEnable()
    {
        // Mỗi lần Panel này được BẬT lên, tự động quét để làm mờ súng theo Map đã chọn
        CheckWeaponAvailability();

        // FIX: báo cho GameStateManager biết panel này đang mở, để phím ESC không mở
        // Pause đè lên panel Chọn Vũ Khí/Màn nữa (bug "ấn pause hiện bảng pause chèn vào").
        if (GameStateManager.Instance != null)
            GameStateManager.Instance.SetLocked(true);
    }

    void OnDisable()
    {
        // FIX: mở khoá lại khi panel này bị tắt đi (ví dụ người chơi bấm Xác Nhận để vào màn)
        if (GameStateManager.Instance != null)
            GameStateManager.Instance.SetLocked(false);
    }

    void CheckWeaponAvailability()
    {
        // Lấy số thứ tự Map mà người chơi vừa ấn từ GameStateManager toàn cục
        int currentMap = GameStateManager.Instance.currentMapIndex;

        foreach (var weapon in allWeapons)
        {
            if (currentMap >= weapon.requiredMap)
            {
                // ĐÃ MỞ KHÓA: Cho phép bấm nút, chữ hiện rõ ràng (Alpha = 1)
                weapon.buttonComponent.interactable = true;
                weapon.buttonText.color = new Color(weapon.buttonText.color.r, weapon.buttonText.color.g, weapon.buttonText.color.b, 1f);
            }
            else
            {
                // CHƯA MỞ KHÓA: Khóa nút, làm mờ chữ đi (Alpha = 0.25f)
                weapon.buttonComponent.interactable = false;
                weapon.buttonText.color = new Color(weapon.buttonText.color.r, weapon.buttonText.color.g, weapon.buttonText.color.b, 0.25f);
            }
        }
    }

    // --- CÁC HÀM GẮN VÀO NÚT BẤM (ON CLICK) ---

    public void SelectMainWeapon(string weaponID)
    {
        GameStateManager.Instance.selectedMainWeapon = weaponID;
        Debug.Log("Đã chọn vũ khí chính: " + weaponID);
    }

    public void SelectMeleeWeapon(string weaponID)
    {
        GameStateManager.Instance.selectedMeleeWeapon = weaponID;
        Debug.Log("Đã chọn vũ khí cận chiến: " + weaponID);
    }

    // Hàm gắn vào nút XÁC NHẬN cuối cùng để bay vào màn chơi
    public void ConfirmAndLoadZone(string sceneNameToLoad)
    {
        SceneManager.LoadScene(sceneNameToLoad);
    }
}