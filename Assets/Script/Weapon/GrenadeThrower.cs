using UnityEngine;

/// <summary>
/// Gắn script này vào GameObject "Player" (cùng chỗ với PlayerController).
/// Ném grenade theo hướng nhìn (forward) của Camera (MouseLook), grenade sẽ
/// tự nổ theo bán kính cố định được cấu hình trong script Grenade.
/// Số lượng lựu đạn được lấy/trừ từ script Inventory (gắn cùng GameObject).
/// </summary>
public class GrenadeThrower : MonoBehaviour
{
    [Header("Grenade")]
    public GameObject grenadePrefab;

    [Tooltip("Camera của người chơi (object có script MouseLook). Dùng để lấy hướng ném")]
    public Transform cameraTransform;

    [Tooltip("Điểm xuất phát của grenade, nên đặt hơi ra phía trước Camera. Có thể để trống")]
    public Transform throwPoint;

    [Header("Inventory")]
    [Tooltip("Kéo Inventory của Player vào đây. Để trống thì tự tìm trên cùng GameObject")]
    public Inventory inventory;

    [Header("Lực ném")]
    public float throwForce = 20f;
    [Tooltip("Thêm 1 chút góc hướng lên để grenade bay theo vòng cung, 0 = ném thẳng theo hướng nhìn")]
    [Range(0f, 1f)]
    public float upwardArc = 0.15f;

    [Header("Hồi chiêu")]
    public float throwCooldown = 1f;

    [Header("Phím ném")]
    public KeyCode throwKey = KeyCode.G;

    [Tooltip("Số lựu đạn còn lại, đọc từ Inventory - dùng để hiển thị UI nếu cần")]
    public int GrenadeCount => inventory != null ? inventory.grenadeCount : 0;

    private float nextThrowTime;
    private Collider playerCollider;

    void Awake()
    {
        playerCollider = GetComponent<Collider>();

        if (inventory == null)
            inventory = GetComponent<Inventory>();

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        if (Input.GetKeyDown(throwKey))
        {
            TryThrow();
        }
    }

    public bool TryThrow()
    {
        if (Time.time < nextThrowTime) return false;
        if (grenadePrefab == null || cameraTransform == null) return false;

        // Hết lựu đạn trong Inventory thì không ném được
        if (inventory != null && !inventory.UseGrenade()) return false;

        Vector3 spawnPos = throwPoint != null
            ? throwPoint.position
            : cameraTransform.position + cameraTransform.forward * 0.6f;

        GameObject obj = Instantiate(grenadePrefab, spawnPos, Quaternion.identity);

        // Bỏ qua va chạm giữa grenade và Player để grenade không va/nổ ngay lúc vừa ném ra
        Collider grenadeCol = obj.GetComponent<Collider>();
        if (grenadeCol != null && playerCollider != null)
        {
            Physics.IgnoreCollision(grenadeCol, playerCollider);
        }

        // Hướng ném theo góc nhìn nhân vật (camera forward), cộng thêm 1 chút vòng cung lên trên
        Vector3 throwDir = (cameraTransform.forward + Vector3.up * upwardArc).normalized;
        Vector3 velocity = throwDir * throwForce;

        Grenade grenade = obj.GetComponent<Grenade>();
        if (grenade != null)
        {
            grenade.Launch(velocity, Random.insideUnitSphere * 4f);
        }
        else
        {
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb != null) rb.linearVelocity = velocity;
        }

        nextThrowTime = Time.time + throwCooldown;
        return true;
    }
}