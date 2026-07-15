using UnityEngine;
using UnityEngine.AI;

namespace Enemies
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Animator))]
    public abstract class EnemyBase : MonoBehaviour
    {
        [Header("Stats (tự nhập riêng cho từng prefab)")]
        public float maxHealth = 100f;
        public float moveSpeed = 3.5f;
        public float damage = 10f;
        public float attackRange = 2f;

        [Tooltip("QUAN TRỌNG: phải >= độ dài clip Attack.anim, nếu không đòn đánh sẽ " +
                 "dồn dập hơn dự tính (Attack trigger có thể được set lại trong lúc " +
                 "animation Attack trước đó chưa kết thúc).")]
        public float attackCooldown = 1.5f;

        [Header("Khoảng cách đứng lại (chống xuyên vào player)")]
        [Tooltip("Tỉ lệ so với attackRange để tính điểm zombie sẽ dừng lại, thay vì " +
                 "đi thẳng vào tâm player. VD attackRange=3, ratio=0.85 -> dừng cách " +
                 "player 2.55m. Giảm số này nếu zombie vẫn đứng hơi xa; tăng nếu vẫn " +
                 "còn xuyên model.")]
        [Range(0.3f, 1f)]
        public float standoffRatio = 0.85f;

        [Tooltip("Chỉ tính lại điểm đứng (destination) mỗi khoảng thời gian này thay vì " +
                 "mỗi frame, để đỡ tốn NavMesh pathfinding khi có nhiều zombie.")]
        public float destinationRefreshInterval = 0.2f;

        [Tooltip("Khoảng cách TỐI THIỂU bắt buộc phải giữ với player (tính từ tâm 2 " +
                 "transform). Nên đặt = bán kính collider player + agent.radius của " +
                 "zombie + 1 chút buffer. Đây là lớp an toàn cuối: dù destination hay " +
                 "stoppingDistance bị trễ/overshoot do lag, moveSpeed cao, hay nhiều " +
                 "zombie đẩy nhau vào player, zombie vẫn KHÔNG BAO GIỜ lấn qua khoảng " +
                 "cách này vì bị đẩy ngược ra mỗi frame.")]
        public float minPlayerSeparation = 1.2f;

        [Header("Phát hiện người chơi")]
        [Tooltip("Để 0 = luôn đuổi theo player ngay từ đầu. Đặt > 0 nếu muốn " +
                 "zombie đứng Idle cho tới khi player vào tầm này.")]
        public float detectRange = 0f;

        [Header("Animator Parameter Names (tự đặt tên khớp với Animator)")]
        public string paramIsWalking = "isWalking";
        public string paramAttackTrigger = "Attack";
        public string paramDieTrigger = "Die";

        [Header("Rớt đồ khi chết")]
        [Tooltip("Prefab bình hồi máu (đang gắn HealthPickup)")]
        public GameObject healthPickupPrefab;
        [Tooltip("Prefab lựu đạn nhặt được (đang gắn GrenadePickup)")]
        public GameObject grenadePickupPrefab;
        [Tooltip("Xác suất rớt đồ khi chết, 0 = không bao giờ rớt, 1 = luôn luôn rớt")]
        [Range(0f, 1f)]
        public float dropChance = 0.5f;

        protected float currentHealth;
        protected Transform player;
        protected NavMeshAgent agent;
        protected Animator animator;
        protected float lastAttackTime;
        protected bool isDead;
        protected bool hasDetectedPlayer;

        public float CurrentHealth => currentHealth;
        public float MaxHealth => maxHealth;
        public bool IsDead => isDead;

        // Cache tính toán hiệu năng
        private float attackRangeSqr;
        private float detectRangeSqr;
        private float standoffDistance;
        private float nextDestinationRefreshTime;

        protected virtual void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();

            // Tự xoay bằng FaceTarget(), tắt để agent không giành quyền xoay -> tránh giật hướng
            agent.updateRotation = false;

            // Random nhẹ avoidancePriority mỗi con để tránh hiện tượng deadlock khi dồn đàn
            agent.avoidancePriority = Random.Range(40, 61);

            // Tự động gắn thanh máu phía trên đầu zombie nếu chưa có
            if (GetComponent<ZombieHealthBar>() == null)
            {
                gameObject.AddComponent<ZombieHealthBar>();
            }
        }

        protected virtual void Start()
        {
            currentHealth = maxHealth;
            agent.speed = moveSpeed;

            attackRangeSqr = attackRange * attackRange;
            detectRangeSqr = detectRange * detectRange;

            // Điểm dừng thực tế trước khi chạm vào player để tránh xung đột với ResolvePlayerOverlap
            standoffDistance = Mathf.Max(minPlayerSeparation + 0.05f, attackRange * standoffRatio);
            agent.stoppingDistance = standoffDistance;

            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }

            hasDetectedPlayer = detectRange <= 0f;
        }

        protected virtual void Update()
        {
            if (isDead || player == null) return;

            Vector3 toPlayer = player.position - transform.position;
            toPlayer.y = 0f;
            float distanceSqr = toPlayer.sqrMagnitude;

            // Xử lý phát hiện người chơi
            if (!hasDetectedPlayer)
            {
                agent.isStopped = true;
                SetWalking(false);

                if (distanceSqr <= detectRangeSqr)
                    hasDetectedPlayer = true;
                else
                    return;
            }

            // Xử lý Tấn công hoặc Đuổi theo
            if (distanceSqr <= attackRangeSqr)
            {
                agent.isStopped = true;
                SetWalking(false);
                TryAttack();
            }
            else
            {
                agent.isStopped = false;
                UpdateChaseDestination(toPlayer);
                SetWalking(true);
            }

            FaceTarget();
            ResolvePlayerOverlap();
        }

        /// <summary>
        /// Lớp an toàn cuối: đẩy zombie ra ngoài nếu bị đè quá sát vào Player
        /// </summary>
        protected virtual void ResolvePlayerOverlap()
        {
            if (player == null || minPlayerSeparation <= 0f) return;

            Vector3 fromPlayer = transform.position - player.position;
            fromPlayer.y = 0f;
            float dist = fromPlayer.magnitude;

            if (dist >= minPlayerSeparation) return;

            Vector3 pushDir = dist > 0.0001f ? fromPlayer / dist : transform.forward;
            float pushAmount = minPlayerSeparation - dist;
            agent.Move(pushDir * pushAmount);
        }

        /// <summary>
        /// Cập nhật điểm di chuyển định kỳ nhằm giảm tải tối đa cho hệ thống Pathfinding
        /// </summary>
        protected virtual void UpdateChaseDestination(Vector3 toPlayer)
        {
            if (Time.time < nextDestinationRefreshTime) return;
            nextDestinationRefreshTime = Time.time + destinationRefreshInterval;

            Vector3 dirFromPlayer = -toPlayer;
            if (dirFromPlayer.sqrMagnitude < 0.0001f)
                dirFromPlayer = transform.forward;

            dirFromPlayer.Normalize();
            Vector3 standPoint = player.position + dirFromPlayer * standoffDistance;

            agent.SetDestination(standPoint);
        }

        /// <summary>
        /// Quay mặt mượt mà về hướng Player bằng Slerp
        /// </summary>
        protected virtual void FaceTarget()
        {
            Vector3 dir = player.position - transform.position;
            dir.y = 0f;
            if (dir.sqrMagnitude < 0.001f) return;

            Quaternion targetRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 8f);
        }

        protected virtual void TryAttack()
        {
            if (Time.time - lastAttackTime < attackCooldown)
                return;

            lastAttackTime = Time.time;

            // Kích hoạt Trigger để chạy Animation. Gây sát thương thực tế sẽ do Animation Event đảm nhận.
            if (animator != null && !string.IsNullOrEmpty(paramAttackTrigger))
                animator.SetTrigger(paramAttackTrigger);
        }

        /// <summary>
        /// Hàm này phải được gọi thông qua Animation Event đặt trong clip Attack.anim
        /// </summary>
        public virtual void AnimEvent_DealDamage()
        {
            if (isDead || player == null) return;

            float distanceSqr = (player.position - transform.position).sqrMagnitude;
            float buffered = attackRange + 0.5f; // Thêm một chút buffer để tránh miss oan khi Player lùi nhẹ
            if (distanceSqr <= buffered * buffered)
                Attack();
        }

        /// <summary>Mỗi loại enemy kế thừa tự quy định phương thức gây damage (Melee/Ranged)</summary>
        protected abstract void Attack();

        protected virtual void SetWalking(bool walking)
        {
            if (animator != null && !string.IsNullOrEmpty(paramIsWalking))
                animator.SetBool(paramIsWalking, walking);
        }

        public virtual void TakeDamage(float amount)
        {
            if (isDead) return;

            currentHealth -= amount;

            if (currentHealth <= 0)
                Die();
        }

        protected virtual void Die()
        {
            isDead = true;
            agent.isStopped = true;
            SetWalking(false);

            if (animator != null && !string.IsNullOrEmpty(paramDieTrigger))
                animator.SetTrigger(paramDieTrigger);

            TryDropItem();
            Destroy(gameObject, 2f); // Hủy GameObject sau 2 giây (đủ thời gian chạy xong Anim chết)
        }

        /// <summary>
        /// Xử lý rớt ngẫu nhiên vật phẩm dựa trên tỷ lệ dropChance
        /// </summary>
        protected virtual void TryDropItem()
        {
            if (Random.value > dropChance) return;

            bool dropHealth = Random.value < 0.5f;
            GameObject prefabToSpawn = dropHealth ? healthPickupPrefab : grenadePickupPrefab;

            // Dự phòng: Nếu loại được chọn chưa gán prefab thì đổi sang loại còn lại
            if (prefabToSpawn == null)
                prefabToSpawn = dropHealth ? grenadePickupPrefab : healthPickupPrefab;

            if (prefabToSpawn != null)
            {
                Instantiate(prefabToSpawn, transform.position, Quaternion.identity);
            }
        }

        protected virtual void OnDrawGizmosSelected()
        {
            if (detectRange > 0f)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(transform.position, detectRange);
            }

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, Mathf.Max(0.1f, attackRange * standoffRatio));

            Gizmos.color = new Color(0.6f, 0f, 0f); // Đỏ đậm: Vùng cấm tuyệt đối
            Gizmos.DrawWireSphere(transform.position, minPlayerSeparation);
        }
    }
}