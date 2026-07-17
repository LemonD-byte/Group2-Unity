using UnityEngine;

/// <summary>
/// Quản lý số Coin của người chơi. Là static class (không phải MonoBehaviour) nên
/// dùng được ở MỌI scene (MainMenu, Map) mà không cần kéo thả GameObject nào cả.
/// Coin được lưu bền vững qua PlayerPrefs -> tắt game mở lại vẫn còn.
/// </summary>
public static class CurrencyManager
{
    private const string CoinKey = "PlayerCoins";

    /// <summary>Bắn ra mỗi khi số coin thay đổi, để UI (coin text) tự cập nhật.</summary>
    public static System.Action<int> OnCoinsChanged;

    public static int Coins
    {
        get => PlayerPrefs.GetInt(CoinKey, 0);
        private set
        {
            int clamped = Mathf.Max(0, value);
            PlayerPrefs.SetInt(CoinKey, clamped);
            PlayerPrefs.Save();
            OnCoinsChanged?.Invoke(clamped);
        }
    }

    public static void AddCoins(int amount)
    {
        if (amount <= 0) return;
        Coins += amount;
    }

    /// <summary>Trừ coin nếu đủ. Trả về false nếu không đủ tiền (không trừ gì cả).</summary>
    public static bool TrySpendCoins(int amount)
    {
        if (amount <= 0) return true;
        if (Coins < amount) return false;
        Coins -= amount;
        return true;
    }
}