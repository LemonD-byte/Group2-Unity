using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Gắn vào GameObject ShopPanel trong scene MainMenu.
/// Mỗi vật phẩm là 1 icon (Image) có gắn sẵn component Button — bấm thẳng vào icon là mua luôn,
/// bên dưới icon là 1 Text hiển thị giá tiền. Không cần tạo thêm nút "Mua" riêng.
/// </summary>
public class ShopManager : MonoBehaviour
{
    [Header("Giá vật phẩm (coin)")]
    public int grenadePrice = 20;
    public int healPotionPrice = 15;

    [Header("Coin - hiển thị ở phần trên cùng Shop")]
    public TMP_Text coinText;

    [Header("Vật phẩm: Bom (Button gắn ngay trên icon)")]
    public Button grenadeButton;
    public TMP_Text grenadePriceText;

    [Header("Vật phẩm: Bình hồi máu (Button gắn ngay trên icon)")]
    public Button healButton;
    public TMP_Text healPriceText;

    [Header("Tuỳ chọn: số lượng đang sở hữu (có thể để trống nếu không cần hiển thị)")]
    public TMP_Text grenadeOwnedText;
    public TMP_Text healOwnedText;

    void OnEnable()
    {
        CurrencyManager.OnCoinsChanged += OnCoinsChanged;

        // Gắn sự kiện bấm bằng code, không cần kéo OnClick() thủ công trong Inspector nữa
        if (grenadeButton != null) grenadeButton.onClick.AddListener(BuyGrenade);
        if (healButton != null) healButton.onClick.AddListener(BuyHealPotion);

        RefreshUI();
    }

    void OnDisable()
    {
        CurrencyManager.OnCoinsChanged -= OnCoinsChanged;

        if (grenadeButton != null) grenadeButton.onClick.RemoveListener(BuyGrenade);
        if (healButton != null) healButton.onClick.RemoveListener(BuyHealPotion);
    }

    void OnCoinsChanged(int newAmount)
    {
        RefreshUI();
    }

    void RefreshUI()
    {
        int coins = CurrencyManager.Coins;

        if (coinText != null) coinText.text = coins.ToString();

        if (grenadePriceText != null) grenadePriceText.text = grenadePrice.ToString();
        if (healPriceText != null) healPriceText.text = healPotionPrice.ToString();

        if (grenadeOwnedText != null) grenadeOwnedText.text = InventoryStock.Grenades.ToString();
        if (healOwnedText != null) healOwnedText.text = InventoryStock.HealPotions.ToString();

        // Làm mờ icon khi không đủ coin, để người chơi biết ngay không cần bấm thử
        if (grenadeButton != null) grenadeButton.interactable = coins >= grenadePrice;
        if (healButton != null) healButton.interactable = coins >= healPotionPrice;
    }

    /// <summary>Gọi tự động khi bấm icon Bom (đã AddListener ở OnEnable)</summary>
    public void BuyGrenade()
    {
        if (!CurrencyManager.TrySpendCoins(grenadePrice))
        {
            Debug.Log("Không đủ coin để mua bom!");
            return;
        }
        InventoryStock.Grenades += 1;
        RefreshUI();
    }

    /// <summary>Gọi tự động khi bấm icon Bình hồi máu (đã AddListener ở OnEnable)</summary>
    public void BuyHealPotion()
    {
        if (!CurrencyManager.TrySpendCoins(healPotionPrice))
        {
            Debug.Log("Không đủ coin để mua bình hồi máu!");
            return;
        }
        InventoryStock.HealPotions += 1;
        RefreshUI();
    }
}