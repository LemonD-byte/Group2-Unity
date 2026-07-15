using System.Collections;
using UnityEngine;
using TMPro;

public class GunSystem : MonoBehaviour
{
    [Header("Gun Stats")]
    public float damage = 20f;
    public float range = 100f;
    public float fireRate = 0.2f;
    private float nextTimeToFire = 0f;

    [Header("Ammo System")]
    public int magazineSize = 6;     // Băng đạn tối đa (Ví dụ: 6 viên cho súng lục)
    public int totalAmmo = 90;        // Tổng số đạn dự trữ mang theo
    private int currentAmmo;          // Số đạn hiện tại trong băng

    public float reloadTime = 1.5f;   // Thời gian nạp đạn (giây)
    private bool isReloading = false; // Trạng thái đang nạp đạn hay không

    [Header("References")]
    public Camera fpsCamera;
    public LayerMask enemyLayer;
    public TextMeshProUGUI ammoText;

    [Header("Bullet (đạn vật lý)")]
    public GameObject bulletPrefab;      // Prefab viên đạn (có Rigidbody + Collider + script Bullet)
    public Transform bulletSpawnPoint;   // Điểm bắn ra đạn (đặt ở đầu nòng súng)
    public float bulletVelocity = 60f;   // Tốc độ bay của đạn
    public float bulletLifeTime = 3f;    // Thời gian tự huỷ nếu đạn không trúng gì

    void Awake()
    {
        // Gán ở Awake (chạy trước OnEnable) để tránh UI hiện sai số 0 chớp nhoáng
        // trong lần đầu tiên object được kích hoạt.
        currentAmmo = magazineSize;
    }

    void Start()
    {
        // [VỊ TRÍ 1]: Cập nhật UI ngay khi vào game
        UpdateAmmoUI();
    }

    void OnEnable()
    {
        // Reset trạng thái nạp đạn nếu người chơi đổi vũ khí giữa chừng
        isReloading = false;
        // Cập nhật lại UI phòng trường hợp đổi súng đạn khác
        UpdateAmmoUI();
    }

    void Update()
    {
        if (isReloading) return; // Nếu đang nạp đạn thì không cho làm gì cả

        // Tự động nạp đạn khi hết đạn mà vẫn cố bấm bắn, hoặc người chơi chủ động bấm phím R
        if ((Input.GetButton("Fire1") && currentAmmo <= 0) || (Input.GetKeyDown(KeyCode.R) && currentAmmo < magazineSize))
        {
            if (totalAmmo > 0) // Còn đạn dự trữ mới cho nạp
            {
                StartCoroutine(Reload());
            }
            return;
        }

        // Logic bắn súng
        if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire && currentAmmo > 0)
        {
            nextTimeToFire = Time.time + fireRate;
            Shoot();
        }
    }

    void Shoot()
    {
        currentAmmo--; // Bắn 1 viên thì trừ 1 viên

        // [VỊ TRÍ 2]: Cập nhật UI ngay sau khi trừ đạn
        UpdateAmmoUI();

        Debug.Log("Đạn trong băng: " + currentAmmo + "/" + magazineSize + " | Dự trữ: " + totalAmmo);

        if (bulletPrefab == null || bulletSpawnPoint == null)
        {
            Debug.LogError("GunSystem: chưa gán bulletPrefab hoặc bulletSpawnPoint trong Inspector!");
            return;
        }

        // Tạo viên đạn thật tại đầu nòng súng, hướng theo hướng camera đang nhìn
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, fpsCamera.transform.rotation);

        // Gán sát thương cho viên đạn vừa tạo (đồng bộ với damage của súng)
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
            bulletScript.bulletDamage = damage;

        // Đẩy đạn bay đi theo hướng camera
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        if (bulletRb != null)
            bulletRb.AddForce(fpsCamera.transform.forward.normalized * bulletVelocity, ForceMode.Impulse);

        // Tự huỷ đạn sau một khoảng thời gian nếu bay mà không trúng gì (tránh rác trong scene)
        Destroy(bullet, bulletLifeTime);

        // Nếu có làm hiệu ứng tia lửa/khói đầu nòng thì thêm ở đây
    }

    IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log(" đang nạp đạn...");

        // Chờ hết thời gian nạp đạn
        yield return new WaitForSeconds(reloadTime);

        // Tính toán số đạn cần nạp vào băng
        int ammoNeeded = magazineSize - currentAmmo;
        int ammoToReload = Mathf.Min(ammoNeeded, totalAmmo);

        currentAmmo += ammoToReload; // Cộng đạn vào băng
        totalAmmo -= ammoToReload;   // Trừ đạn ở kho dự trữ

        isReloading = false;

        // [VỊ TRÍ 3]: Cập nhật UI sau khi nạp đạn xong xuôi
        UpdateAmmoUI();

        Debug.Log("Nạp đạn xong! Đạn hiện tại: " + currentAmmo + "/" + magazineSize);
    }

    public void UpdateAmmoUI()
    {
        // CHỈ cho phép cập nhật UI nếu khẩu súng này ĐANG ĐƯỢC CẦM TRÊN TAY (Active)
        if (gameObject.activeInHierarchy && ammoText != null)
        {
            ammoText.text = currentAmmo + " / " + totalAmmo;
        }
    }
}