using UnityEngine;

namespace Enemies
{
    /// <summary>
    /// Hulk Zombie (Boss) - bản đơn giản, CHỈ dùng 3 animation: walk, attack01, die.
    ///
    /// Không có state Idle riêng: khi đứng yên (chưa tới lượt đuổi theo hoặc đang trong tầm
    /// đánh), animation "walk" được giữ nguyên nhưng đóng băng lại (tốc độ phát = 0) qua
    /// Float parameter "WalkSpeed" — thay vì chuyển qua 1 state Idle riêng như zombie thường
    /// (zombie thường có clip Idle riêng nên dùng Bool "isWalking" để chuyển giữa 2 state;
    /// Hulk không có clip Idle nên đổi cách điều khiển cho phù hợp).
    ///
    /// KHÔNG cần sửa gì trong EnemyBase — SetWalking() vốn đã là hàm virtual, override ngay
    /// tại đây là đủ.
    /// </summary>
    public class HulkZombie : EnemyBase
    {
        [Header("Đòn đánh")]
        public float knockbackForce;

        [Header("Animator - riêng cho Hulk")]
        [Tooltip("Tên Float parameter điều khiển tốc độ phát animation Walk " +
                 "(0 = đứng yên/đóng băng, 1 = đi bình thường)")]
        private string paramWalkSpeed = "WalkSpeed";

        /// <summary>
        /// Override lại cách EnemyBase báo hiệu đang di chuyển hay không: thay vì SetBool
        /// giữa 2 state Idle/Walk (Hulk không có state Idle riêng), chỉ đổi tốc độ phát của
        /// chính state Walk thông qua SetFloat.
        /// </summary>
        protected override void SetWalking(bool walking)
        {
            if (animator != null && !string.IsNullOrEmpty(paramWalkSpeed))
                animator.SetFloat(paramWalkSpeed, walking ? 1f : 0f);
        }

        protected override void Attack()
        {
            if (player == null) return;

            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            Rigidbody rb = player.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 dir = (player.position - transform.position).normalized;
                rb.AddForce(dir * knockbackForce, ForceMode.Impulse);
            }
        }
    }
}