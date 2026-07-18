using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{
    public float baseDamage = 35f;
    public float attackRate = 0.5f;
    public float attackRange = 2f;
    public LayerMask enemyLayer;
    private float nextAttackTime = 0f;

    void Update()
    {
        if (Input.GetButtonDown("Fire1") && Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + attackRate;
            Attack();
        }
    }

    void Attack()
    {
        // Hiệu ứng Swing ở đây nếu có anim
        
        // Tính tổng sát thương sau khi cộng thuốc từ Shop
        float finalDamage = baseDamage * GameStateManager.Instance.damageMultiplier;

        // Quét hình cầu phía trước xem có trúng Zombie không
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, attackRange, enemyLayer))
        {
            Enemies.EnemyBase enemy = hit.collider.GetComponentInParent<Enemies.EnemyBase>();
            if (enemy != null)
            {
                enemy.TakeDamage(finalDamage);
                Debug.Log("Melee Đánh trúng Zombie! Sát thương: " + finalDamage);
            }
        }
    }
}