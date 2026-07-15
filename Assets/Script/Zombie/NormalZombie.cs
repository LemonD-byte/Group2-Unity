using UnityEngine;

namespace Enemies
{
    /// <summary>
    /// Zombie thường: đi bộ tới người chơi và tấn công cận chiến.
    /// Không cần override gì thêm — toàn bộ Idle/Chase/Attack + Animator
    /// đã được EnemyBase xử lý.
    /// </summary>
    public class NormalZombie : EnemyBase
    {
        protected override void Attack()
        {
            if (player == null) return;

            var playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
                playerHealth.TakeDamage(damage);
        }
    }
}