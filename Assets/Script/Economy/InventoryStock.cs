using UnityEngine;

/// <summary>
/// Kho hàng ĐÃ MUA của người chơi (bom, bình hồi máu), lưu bền vững qua PlayerPrefs.
/// ShopManager (ở MainMenu) ghi vào đây khi mua đồ.
/// Inventory (ở scene chơi, gắn trên Player) đọc từ đây lúc Start và ghi lại khi dùng đồ,
/// để việc mua/dùng luôn đồng bộ qua lại giữa 2 scene.
/// </summary>
public static class InventoryStock
{
    private const string GrenadeKey = "Stock_Grenade";
    private const string HealKey = "Stock_HealPotion";

    public static int Grenades
    {
        get => PlayerPrefs.GetInt(GrenadeKey, 0);
        set { PlayerPrefs.SetInt(GrenadeKey, Mathf.Max(0, value)); PlayerPrefs.Save(); }
    }

    public static int HealPotions
    {
        get => PlayerPrefs.GetInt(HealKey, 0);
        set { PlayerPrefs.SetInt(HealKey, Mathf.Max(0, value)); PlayerPrefs.Save(); }
    }
}