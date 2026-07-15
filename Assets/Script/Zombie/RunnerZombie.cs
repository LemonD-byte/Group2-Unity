using UnityEngine;

namespace Enemies
{
    /// <summary>
    /// Zombie chạy nhanh: lao vào người chơi với tốc độ cao khi đến gần.
    /// ANIMATION: dùng chung 3 bool isWalking/isChasing/isAttacking như mọi
    /// zombie khác — chỉ khác clip "Run.anim" gán ở Attack_Chase state trong
    /// Animator Controller riêng (hoặc Override Controller) của prefab Runner.
    /// </summary>
    public class RunnerZombie : EnemyBase
    {
        [Header("Thông số riêng của Runner (tự nhập)")]
        public float chargeSpeedMultiplier;
        public float chargeTriggerDistance;

        private bool isCharging = false;

        protected override void Update()
        {
            if (isDead || player == null) return;

            float distance = Vector3.Distance(transform.position, player.position);

            // Tăng tốc lao khi vào tầm kích hoạt
            if (distance <= chargeTriggerDistance && !isCharging)
            {
                isCharging = true;
                agent.speed = moveSpeed * chargeSpeedMultiplier;
            }
            else if (distance > chargeTriggerDistance && isCharging)
            {
                isCharging = false;
                agent.speed = moveSpeed;
            }

            base.Update();
        }

        protected override void Attack()
        {
            if (player == null) return;

            var playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
                playerHealth.TakeDamage(damage);
        }
    }
}