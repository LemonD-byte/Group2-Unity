using UnityEngine;
using UnityEngine.Serialization;

namespace Enemies
{
    /// <summary>
    /// Zombie Spitter: đứng ở khoảng cách xa (attackRange lớn) và bắn tia đạn (ZombieBullet)
    /// gây sát thương theo nhịp thay vì đấm cận chiến. Toàn bộ Idle/Chase/Animation dùng chung
    /// EnemyBase + Animator Controller y hệt Zombie thường -> không cần chỉnh Animator gì thêm.
    /// </summary>
    public class SpitterZombie : EnemyBase
    {
        [Header("Spitter - Tấn công tầm xa (tia đạn)")]
        [Tooltip("Prefab tia đạn (đang gắn ZombieBullet) — KHÔNG phải chính con zombie")]
        [FormerlySerializedAs("poisonProjectilePrefab")]
        public GameObject bulletPrefab;

        [Tooltip("Điểm bắn ra (kéo 1 Empty GameObject đặt gần miệng zombie vào đây). Để trống sẽ bắn từ giữa người zombie.")]
        public Transform firePoint;

        public float projectileSpeed = 20f;

        protected override void Attack()
        {
            if (bulletPrefab == null || player == null) return;

            Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position + Vector3.up * 1.5f;
            Vector3 targetPos = player.position + Vector3.up * 1f;
            Vector3 dir = (targetPos - spawnPos).normalized;

            GameObject proj = Instantiate(bulletPrefab, spawnPos, Quaternion.LookRotation(dir));
            ZombieBullet bullet = proj.GetComponent<ZombieBullet>();
            if (bullet != null)
                bullet.Init(dir, projectileSpeed);
        }
    }
}