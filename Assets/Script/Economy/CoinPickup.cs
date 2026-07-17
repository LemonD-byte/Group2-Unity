using UnityEngine;

/// <summary>
/// Gắn vào prefab đồng Coin. Collider PHẢI bật Is Trigger.
/// Player chạm vào là tự nhặt (OnTriggerEnter), không cần bấm phím.
/// </summary>
[RequireComponent(typeof(Collider))]
public class CoinPickup : MonoBehaviour
{
    [Tooltip("Số coin nhận được khi nhặt. EnemyBase sẽ random giá trị này lúc Instantiate.")]
    public int value = 1;

    [Header("Hiệu ứng (tuỳ chọn)")]
    public GameObject pickupEffect;
    public AudioClip pickupSound;

    private void Reset()
    {
        var col = GetComponent<Collider>();
        if (col != null) col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        CurrencyManager.AddCoins(value);

        if (pickupEffect != null)
            Instantiate(pickupEffect, transform.position, Quaternion.identity);
        if (pickupSound != null)
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);

        Destroy(gameObject);
    }
}