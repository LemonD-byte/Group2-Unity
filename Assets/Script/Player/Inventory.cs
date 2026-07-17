using UnityEngine;

/// <summary>
/// Gắn script này vào GameObject "Player".
/// Mặc định 0 lựu đạn / 0 bình hồi máu — chỉ có được sau khi MUA ở Shop (MainMenu).
/// Tự đồng bộ 2 chiều với InventoryStock (PlayerPrefs) để số lượng mua ở Shop
/// mang được sang scene chơi, và dùng hết trong scene chơi cũng trừ đúng vào kho.
/// </summary>
public class Inventory : MonoBehaviour
{
    [Header("Lựu đạn (chỉ đọc lúc Play, nguồn thật là InventoryStock)")]
    public int grenadeCount;

    [Header("Bình hồi máu (chỉ đọc lúc Play, nguồn thật là InventoryStock)")]
    public int healPotionCount;

    public bool HasGrenade => grenadeCount > 0;
    public bool HasHealPotion => healPotionCount > 0;

    void Awake()
    {
        // Nạp đúng số lượng đã mua ở Shop vào lượt chơi này
        grenadeCount = InventoryStock.Grenades;
        healPotionCount = InventoryStock.HealPotions;
    }

    /// <summary>Trừ 1 lựu đạn nếu còn. Trả về false nếu đã hết.</summary>
    public bool UseGrenade()
    {
        if (grenadeCount <= 0) return false;
        grenadeCount--;
        InventoryStock.Grenades = grenadeCount;
        return true;
    }

    /// <summary>Trừ 1 bình hồi máu nếu còn. Trả về false nếu đã hết.</summary>
    public bool UseHealPotion()
    {
        if (healPotionCount <= 0) return false;
        healPotionCount--;
        InventoryStock.HealPotions = healPotionCount;
        return true;
    }

    public void AddGrenade(int amount = 1)
    {
        grenadeCount += amount;
        InventoryStock.Grenades = grenadeCount;
    }

    public void AddHealPotion(int amount = 1)
    {
        healPotionCount += amount;
        InventoryStock.HealPotions = healPotionCount;
    }
}