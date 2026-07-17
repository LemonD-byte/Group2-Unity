using UnityEngine;
using System.Collections;

namespace Enemies
{
    /// <summary>
    /// Tia đạn của Zombie Spitter. Bay thẳng tới người chơi; khi va chạm sẽ gây sát thương
    /// theo 3 NHỊP (mặc định), mỗi nhịp cách nhau tickInterval giây, mỗi nhịp mất tickDamage máu
    /// (mặc định 10 máu/nhịp -> tổng 30 máu nếu trúng đủ 3 nhịp).
    /// Thay thế cho PoisonProjectile (không còn để lại vũng độc trên đất nữa).
    ///
    /// YÊU CẦU SETUP TRÊN PREFAB:
    /// - Collider: bật "Is Trigger" = true (nếu không OnTriggerEnter sẽ không bao giờ chạy).
    /// - Rigidbody: tắt "Use Gravity", bật "Is Kinematic" = true (script tự điều khiển di chuyển
    ///   bằng transform, không cần và không nên để vật lý can thiệp).
    /// </summary>
    public class ZombieBullet : MonoBehaviour
    {
        [Header("Di chuyển")]
        [Tooltip("Tự huỷ viên đạn sau chừng này giây nếu bay mà không trúng gì (tránh rác trong scene)")]
        public float lifeTime = 5f;

        [Header("Sát thương theo nhịp")]
        [Tooltip("Số nhịp gây sát thương sau khi trúng người chơi")]
        public int tickCount = 3;
        [Tooltip("Sát thương mỗi nhịp")]
        public float tickDamage = 10f;
        [Tooltip("Khoảng cách thời gian giữa 2 nhịp liên tiếp (giây)")]
        public float tickInterval = 0.3f;

        private Vector3 direction;
        private float speed;
        private bool hasHit;

        /// <summary>Gọi từ SpitterZombie ngay sau khi Instantiate.</summary>
        public void Init(Vector3 dir, float spd)
        {
            direction = dir;
            speed = spd;
            Destroy(gameObject, lifeTime);
        }

        private void Update()
        {
            if (hasHit) return;
            transform.position += direction * speed * Time.deltaTime;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (hasHit) return;
            if (!other.CompareTag("Player")) return;

            hasHit = true;

            PlayerHealth health = other.GetComponent<PlayerHealth>();
            StartCoroutine(DealDamageOverTicks(health));

            // Ẩn model + tắt Collider ngay khi trúng, tránh bắn/va chạm thêm lần nữa
            // trong lúc đang dồn 3 nhịp sát thương.
            SetVisualAndCollisionEnabled(false);
        }

        private IEnumerator DealDamageOverTicks(PlayerHealth health)
        {
            for (int i = 0; i < tickCount; i++)
            {
                if (health != null)
                    health.TakeDamage(tickDamage);

                if (i < tickCount - 1)
                    yield return new WaitForSeconds(tickInterval);
            }

            Destroy(gameObject);
        }

        private void SetVisualAndCollisionEnabled(bool value)
        {
            Collider col = GetComponent<Collider>();
            if (col != null) col.enabled = value;

            Renderer[] renderers = GetComponentsInChildren<Renderer>();
            foreach (Renderer r in renderers) r.enabled = value;
        }
    }
}