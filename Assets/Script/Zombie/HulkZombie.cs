using UnityEngine;

namespace Enemies
{
    /// <summary>
    /// Hulk Zombie — Boss cuối game. Model/rig/animation lấy từ asset glTF hoàn toàn khác
    /// zombie thường (clip có sẵn: walk, run, die, attack01, attack02).
    ///
    /// QUAN TRỌNG: KHÔNG cần sửa gì trong EnemyBase để dùng model khác. EnemyBase đã tách
    /// riêng TÊN parameter Animator (paramIsWalking / paramAttackTrigger / paramDieTrigger)
    /// thành field cấu hình được trong Inspector của từng prefab — chỉ cần tạo 1 Animator
    /// Controller MỚI cho riêng Hulk, đặt tên 3 parameter đó (giữ mặc định "isWalking" /
    /// "Attack" / "Die" cho đơn giản) là chạy được ngay, dù animation clip gốc tên gì cũng
    /// được vì Controller chỉ tham chiếu qua parameter, không quan tâm tên clip.
    ///
    /// Class này CHỈ chứa 2 thứ đặc thù của riêng boss (không có ở zombie thường), để không
    /// làm phình EnemyBase dùng chung cho mọi loại zombie khác:
    ///
    /// 1) Đòn đánh đổi ngẫu nhiên giữa 2 animation attack01/attack02 (tận dụng đúng 2 clip có
    ///    sẵn trong model) — qua Int parameter "AttackType" (0/1), CHỈ cần tạo thêm parameter
    ///    này trong Animator Controller của Hulk, zombie thường không cần biết tới.
    /// 2) Enrage (nổi điên) khi máu xuống thấp: tăng tốc độ + đổi animation đi bộ "walk" sang
    ///    chạy "run" (clip có sẵn nhưng EnemyBase không dùng tới) — qua Bool "IsEnraged".
    /// </summary>
    public class HulkZombie : EnemyBase
    {
        [Header("Đòn đánh")]
        public bool instantKillOnHit = true;
        public float knockbackForce = 15f;

        [Header("Animator - riêng cho Hulk (không có ở EnemyBase / zombie thường)")]
        [Tooltip("Tên Int parameter chọn animation đánh: 0 = attack01, 1 = attack02")]
        public string paramAttackType = "AttackType";
        [Tooltip("Tên Bool parameter báo hiệu đang Enrage (đổi Walk -> Run)")]
        public string paramEnraged = "IsEnraged";

        [Header("Enrage (nổi điên khi máu thấp)")]
        [Range(0f, 1f)]
        [Tooltip("Enrage khi máu hiện tại <= tỉ lệ này so với máu tối đa")]
        public float enrageHealthThreshold = 0.3f;
        [Tooltip("Nhân tốc độ di chuyển khi Enrage (áp lên agent.speed)")]
        public float enrageSpeedMultiplier = 1.6f;

        private bool isEnraged;

        /// <summary>
        /// Gọi hàm gốc ở EnemyBase để xử lý cooldown + trigger "Attack" như bình thường;
        /// nếu đòn đánh THỰC SỰ được tung ra (không còn đang hồi chiêu), chọn thêm ngẫu
        /// nhiên attack01/attack02 để boss không đánh lặp 1 kiểu hoài gây nhàm.
        /// </summary>
        protected override bool TryAttack()
        {
            bool didAttack = base.TryAttack();

            if (didAttack && animator != null && !string.IsNullOrEmpty(paramAttackType))
            {
                int attackType = Random.value < 0.5f ? 0 : 1;
                animator.SetInteger(paramAttackType, attackType);
            }

            return didAttack;
        }

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

            if (!isEnraged && !isDead && currentHealth <= maxHealth * enrageHealthThreshold)
                EnterEnrage();
        }

        private void EnterEnrage()
        {
            isEnraged = true;

            if (agent != null)
                agent.speed = moveSpeed * enrageSpeedMultiplier;

            if (animator != null && !string.IsNullOrEmpty(paramEnraged))
                animator.SetBool(paramEnraged, true);

            Debug.Log("Hulk Zombie nổi điên!");
        }
    }
}