using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [Header("Weapon Slots")]
    public GameObject[] mainWeapons;   // Slot 1: AK47 và Shotgun
    public GameObject[] pistols;       // Slot 2: Súng lục
    public GameObject[] melees;        // Slot 3: Gậy bóng chày

    [Header("Unlock States")]
    public bool isMainUnlocked = false; // Mới vào game: KHÓA AK/Shotgun
    public bool isPistolUnlocked = true;// Mới vào game: MỞ SÚNG LỤC
    public bool isMeleeUnlocked = true; // Mới vào game: MỞ GẬY BÓNG CHÀY

    private int currentMainIndex = 0;
    private int activeSlot = 3;        // Mặc định vào game cầm sẵn Pistol (Slot 1)

    void Start()
    {
        SelectWeaponSlot();
        UpdateWeaponIconUI();
    }

    void Update()
    {
        // Phím 1: Vũ khí chính (AK/Shotgun)
        if (Input.GetKeyDown(KeyCode.Alpha1) && isMainUnlocked && mainWeapons != null && mainWeapons.Length > 0)
        {
            // NẾU đang cầm sẵn Súng chính -> Ấn 1 phát nữa để CẤT VỀ TAY KHÔNG (Slot 3)
            if (activeSlot == 0)
            {
                activeSlot = 3; // Về tay không
            }
            else
            {
                activeSlot = 0; // Nếu đang cầm cái khác hoặc tay không -> Rút súng chính
            }
            SelectWeaponSlot();
        }
        
        // Phím 2: Súng lục
        if (Input.GetKeyDown(KeyCode.Alpha2) && isPistolUnlocked && pistols != null && pistols.Length > 0)
        {
            // NẾU đang cầm sẵn Súng lục -> Ấn 2 phát nữa để CẤT VỀ TAY KHÔNG (Slot 3)
            if (activeSlot == 1)
            {
                activeSlot = 3; // Về tay không
            }
            else
            {
                activeSlot = 1; // Rút súng lục
            }
            SelectWeaponSlot();
        }

        // Phím 3: Cận chiến (Gậy)
        if (Input.GetKeyDown(KeyCode.Alpha3) && isMeleeUnlocked && melees != null && melees.Length > 0)
        {
            // NẾU đang cầm sẵn Gậy -> Ấn 3 phát nữa để CẤT VỀ TAY KHÔNG (Slot 3)
            if (activeSlot == 2)
            {
                activeSlot = 3; // Về tay không
            }
            else
            {
                activeSlot = 2; // Rút gậy
            }
            SelectWeaponSlot();
        }
    }

    void SelectWeaponSlot()
    {
        DeactivateAll();

        if (activeSlot == 3)
        {
            DisableAllAmmoTexts(); // Tắt UI đạn[cite: 2]
            UpdateWeaponIconUI();  // Đổi sang Icon Tay không
            return;
        }

        GameObject activeWeapon = null;

        if (activeSlot == 0 && mainWeapons != null && mainWeapons.Length > 0)
            activeWeapon = mainWeapons[currentMainIndex];
        else if (activeSlot == 1 && pistols != null && pistols.Length > 0)
            activeWeapon = pistols[0];
        else if (activeSlot == 2 && melees != null && melees.Length > 0)
            activeWeapon = melees[0];

        if (activeWeapon != null)
        {
            // Bật Object cha
            activeWeapon.SetActive(true);
            
            // ÉP tất cả mô hình con (kể cả đang xám xịt) phải sáng lên
            for (int i = 0; i < activeWeapon.transform.childCount; i++)
            {
                activeWeapon.transform.GetChild(i).gameObject.SetActive(true);
            }

            HandleAmmoUIToggle(activeWeapon);
        }

        UpdateWeaponIconUI();
    }

    void DeactivateAll()
    {
        if (mainWeapons != null) foreach (GameObject w in mainWeapons) if (w != null) { w.SetActive(false); ToggleChildren(w, false); }
        if (pistols != null)     foreach (GameObject w in pistols)     if (w != null) { w.SetActive(false); ToggleChildren(w, false); }
        if (melees != null)      foreach (GameObject w in melees)      if (w != null) { w.SetActive(false); ToggleChildren(w, false); }
    }

    void ToggleChildren(GameObject parent, bool state)
    {
        for (int i = 0; i < parent.transform.childCount; i++)
        {
            parent.transform.GetChild(i).gameObject.SetActive(state);
        }
    }

    void HandleAmmoUIToggle(GameObject activeWeapon)
    {
        if (activeWeapon == null) return;

        GunSystem gun = activeWeapon.GetComponent<GunSystem>();
        
        if (gun != null)
        {
            // Nếu là súng -> Hiện UI đạn của chính nó lên
            if (gun.ammoText != null) gun.ammoText.gameObject.SetActive(true);
        }
        else
        {
            // BẢN SỬA LỖI AN TOÀN: Nếu là Gậy -> Quét mảng tắt sạch UI đạn, không gọi pistol[0] trực tiếp nữa
            DisableAllAmmoTexts();
        }
    }

    void DisableAllAmmoTexts()
    {
        if (mainWeapons != null) foreach (GameObject w in mainWeapons) if (w != null && w.GetComponent<GunSystem>() != null && w.GetComponent<GunSystem>().ammoText != null) w.GetComponent<GunSystem>().ammoText.gameObject.SetActive(false);
        if (pistols != null)     foreach (GameObject w in pistols)     if (w != null && w.GetComponent<GunSystem>() != null && w.GetComponent<GunSystem>().ammoText != null) w.GetComponent<GunSystem>().ammoText.gameObject.SetActive(false);
    }

    // --- ĐỔI TỪ IMAGE SANG TEXTMESHPRO ---
    [Header("Giao diện UI Tên Vũ khí (Mạnh sửa ở đây)")]
    public TMPro.TextMeshProUGUI weaponNameText; // Kéo ô TextMeshPro hiển thị tên súng vào đây

    void UpdateWeaponIconUI()
    {
        if (weaponNameText == null) return;

        // Thay vì gán ảnh, tụi mình ép chữ hiển thị trực quan luôn!
        if (activeSlot == 0) weaponNameText.text = "VŨ KHÍ CHÍNH (AK47)";
        else if (activeSlot == 1) weaponNameText.text = "SÚNG LỤC (PISTOL)";
        else if (activeSlot == 2) weaponNameText.text = "VŨ KHÍ CẬN CHIẾN (GẬY)";
        else if (activeSlot == 3) weaponNameText.text = "TAY KHÔNG";
    }
}