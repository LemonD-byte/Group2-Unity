using UnityEngine;

namespace Enemies
{
    /// <summary>
    /// Viên đạn độc do Spitter Zombie bắn ra. Khi va chạm sẽ gây sát thương
    /// và để lại 1 bãi dịch độc (poison pool) trên mặt đất.
    /// Gắn script này vào prefab đạn (kèm Collider dạng Trigger + Rigidbody Kinematic).
    /// </summary>
    public class PoisonProjectile : MonoBehaviour
    {
        [Header("Tham chiếu")]
        public GameObject poisonPoolPrefab; // prefab bãi dịch độc để lại trên đất
        public float lifeTime = 5f;
        public float poolDuration = 5f;

        private Vector3 direction;
        private float speed;
        private float damage;

        public void Init(Vector3 dir, float spd, float dmg)
        {
            direction = dir;
            speed = spd;
            damage = dmg;
            Destroy(gameObject, lifeTime);
        }

        private void Update()
        {
            transform.position += direction * speed * Time.deltaTime;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                var health = other.GetComponent<PlayerHealth>();
                if (health != null)
                    health.TakeDamage(damage);
            }

            SpawnPoisonPool();
            Destroy(gameObject);
        }

        private void SpawnPoisonPool()
        {
            if (poisonPoolPrefab == null) return;

            GameObject pool = Instantiate(poisonPoolPrefab, transform.position, Quaternion.identity);
            Destroy(pool, poolDuration);
        }
    }
}
