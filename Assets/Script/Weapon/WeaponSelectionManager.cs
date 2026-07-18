using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class WeaponSelectionManager : MonoBehaviour
{
    [System.Serializable]
    public class WeaponUIInfo
    {
        public string weaponName;     
        public int requiredLevelIndex;  
        public Button selectionButton;  

        public TextMeshProUGUI txtStats;
        [TextArea(2, 5)] public string statsText; 
    }

    [Header("--- DANH SÁCH VŨ KHÍ ---")]
    public WeaponUIInfo[] weapons;

    [Header("--- CẤU HÌNH MÀU BLOCK ---")]
    public Color lockedColor = new Color(0.3f, 0.3f, 0.3f, 1f);
    public Color unlockedColor = Color.white;                  

    [Header("--- NÚT BẮT ĐẦU VÀO TRẬN ---")]
    public Button btnStartGame;
    
    private string selectedWeaponName = "Pistol"; 

    void Start()
    {
        RefreshWeaponSelectionUI();

        if (btnStartGame != null)
        {
            btnStartGame.onClick.AddListener(SaveWeaponSelection);
        }
    }

    void RefreshWeaponSelectionUI()
    {
        int highestClearedLevel = PlayerPrefs.GetInt("HighestClearedLevel", 0);

        for (int i = 0; i < weapons.Length; i++)
        {
            WeaponUIInfo w = weapons[i];

            bool isUnlocked = (highestClearedLevel >= w.requiredLevelIndex);

            if (isUnlocked)
            {
                w.txtStats.text = w.statsText;      
                w.selectionButton.interactable = true;

                if (w.selectionButton.image != null)
                {
                    w.selectionButton.image.color = unlockedColor;
                }

                string currentWeaponName = w.weaponName;
                w.selectionButton.onClick.RemoveAllListeners();
                w.selectionButton.onClick.AddListener(() => SelectWeapon(currentWeaponName));
            }
            else
            {
                w.txtStats.text = "<color=red><size=16>CHƯA MỞ KHÓA KHÓA</color>\n" + w.statsText; 
                w.selectionButton.interactable = false; 

                if (w.selectionButton.image != null)
                {
                    w.selectionButton.image.color = lockedColor;

                    ColorBlock cb = w.selectionButton.colors;
                    cb.disabledColor = lockedColor;
                    w.selectionButton.colors = cb;
                }
            }
        }
    }

    void SelectWeapon(string name)
    {
        selectedWeaponName = name;
        Debug.Log("Đã chọn vũ khí mang vào trận: " + selectedWeaponName);
        PlayerPrefs.SetString("SelectedWeapon", selectedWeaponName);
    }

    public void SaveWeaponSelection()
    {
        PlayerPrefs.SetString("SelectedWeapon", selectedWeaponName);
        PlayerPrefs.Save();
        
        Debug.Log("Đã lưu vũ khí: " + selectedWeaponName + "! Sẵn sàng chuyển sang MapSelectPanel.");
    }
}