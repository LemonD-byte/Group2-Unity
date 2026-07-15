using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float bulletDamage;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemies"))
        {
            // GetComponentInParent để không bị miss khi Collider nằm ở model con
            // (đồng nhất với cách MeleeWeapon đang lấy EnemyBase).
            Enemies.EnemyBase enemy = collision.gameObject.GetComponentInParent<Enemies.EnemyBase>();
            if (enemy != null)
            {
                enemy.TakeDamage(bulletDamage); // Zombie mất máu, hết máu sẽ tự Die() trong EnemyBase
            }
        }

        // Đạn trúng bất kỳ vật gì (zombie, tường, đất...) đều biến mất, không xuyên qua
        Destroy(gameObject);
    }
}