using UnityEngine;

/// <summary>
/// Gắn script này vào GameObject "Player".
/// Giữ số lượng vật phẩm dùng được (lựu đạn, bình hồi máu) tại 1 chỗ duy nhất.
/// GrenadeThrower và PlayerHealth sẽ tự tìm component này (GetComponent) nếu
/// cùng nằm trên Player, không cần kéo tay nếu đặt chung 1 GameObject.
/// </summary>
public class Inventory : MonoBehaviour
{
    [Header("Lựu đạn")]
    [Tooltip("Số lượng lựu đạn hiện có. -1 = không giới hạn")]
    public int grenadeCount = 3;

    [Header("Bình hồi máu")]
    [Tooltip("Số lượng bình hồi máu hiện có. -1 = không giới hạn")]
    public int healPotionCount = 3;

    public bool HasGrenade => grenadeCount != 0;
    public bool HasHealPotion => healPotionCount != 0;

    /// <summary>Trừ 1 lựu đạn nếu còn. Trả về false nếu đã hết.</summary>
    public bool UseGrenade()
    {
        if (grenadeCount == 0) return false;
        if (grenadeCount > 0) grenadeCount--;
        return true;
    }

    /// <summary>Trừ 1 bình hồi máu nếu còn. Trả về false nếu đã hết.</summary>
    public bool UseHealPotion()
    {
        if (healPotionCount == 0) return false;
        if (healPotionCount > 0) healPotionCount--;
        return true;
    }

    /// <summary>Gọi khi nhặt được lựu đạn ngoài map (VD: GrenadePickup).</summary>
    public void AddGrenade(int amount = 1)
    {
        if (grenadeCount < 0) return; // đang không giới hạn thì khỏi cộng thêm
        grenadeCount += amount;
    }

    /// <summary>Gọi khi nhặt được bình hồi máu ngoài map (VD: HealthPickup).</summary>
    public void AddHealPotion(int amount = 1)
    {
        if (healPotionCount < 0) return;
        healPotionCount += amount;
    }
}