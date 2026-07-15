using UnityEngine;

namespace Enemies
{
    /// <summary>
    /// Hulk Zombie (Boss): máu cực lớn, di chuyển chậm, đòn đánh có thể
    /// "one-shot" người chơi. ANIMATION: dùng chung bool "isAttacking" như
    /// mọi zombie khác — animation "đấm mạnh" khác biệt chỉ cần đổi clip gán
    /// vào Attack_State trong Animator Controller riêng của prefab Hulk,
    /// không cần trigger riêng trong code.
    /// </summary>
    public class HulkZombie : EnemyBase
    {
        [Header("Thông số riêng của Hulk (tự nhập)")]
        public bool instantKillOnHit = true;
        public float knockbackForce;

        protected override void Attack()
        {
            if (player == null) return;

            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                if (instantKillOnHit)
                    playerHealth.Kill();
                else
                    playerHealth.TakeDamage(damage);
            }

            Rigidbody rb = player.GetComponent<Rigidbody>();

            if (rb != null)
            {
                Vector3 dir = (player.position - transform.position).normalized;
                rb.AddForce(dir * knockbackForce, ForceMode.Impulse);
            }
        }

        public override void TakeDamage(float amount)
        {
            base.TakeDamage(amount);
            // TODO: hiệu ứng giận dữ / tăng tốc khi máu thấp, v.v.
        }
    }
}