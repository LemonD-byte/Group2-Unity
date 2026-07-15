using UnityEngine;

namespace Enemies
{
    /// <summary>
    /// Gắn vào prefab bãi dịch độc (poison pool). Gây sát thương theo thời gian
    /// (damage per tick) cho người chơi khi đứng trong vùng độc.
    /// Cần Collider dạng Trigger.
    /// </summary>
    public class PoisonPool : MonoBehaviour
    {
        public float damagePerTick = 5f;
        public float tickInterval = 1f;

        private float tickTimer;
        private bool playerInside = false;
        private PlayerHealth playerHealth;

        private void Update()
        {
            if (!playerInside || playerHealth == null) return;

            tickTimer += Time.deltaTime;
            if (tickTimer >= tickInterval)
            {
                tickTimer = 0f;
                playerHealth.TakeDamage(damagePerTick);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                playerInside = true;
                playerHealth = other.GetComponent<PlayerHealth>();
                tickTimer = tickInterval; // gây damage ngay khi bước vào
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                playerInside = false;
                playerHealth = null;
            }
        }
    }
}
