using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Grenade : MonoBehaviour
{
    [Header("Ngòi nổ")]
    public float fuseTime;

    [Header("Vùng nổ")]
    public float explosionRadius;
    public float explosionDamage;

    [Tooltip("Chỉ các Layer được chọn mới nhận sát thương")]
    [SerializeField] private LayerMask damageLayer;

    [Header("Player")]
    public bool damagePlayer = true;

    [Header("Hiệu ứng")]
    public GameObject explosionEffectPrefab;

    [Header("Nổ khi va chạm")]
    public bool explodeOnZombieContact = true;

    private Rigidbody rb;
    private bool hasExploded;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        Invoke(nameof(Explode), fuseTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!explodeOnZombieContact)
            return;

        if (collision.collider.GetComponentInParent<Enemies.EnemyBase>() != null)
        {
            Explode();
        }
    }

    public void Launch(Vector3 velocity, Vector3 angularVelocity)
    {
        rb.linearVelocity = velocity;
        rb.angularVelocity = angularVelocity;
    }

    void Explode()
    {
        if (hasExploded)
            return;
        hasExploded = true;
        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            explosionRadius,
            damageLayer,
            QueryTriggerInteraction.Collide);

        System.Collections.Generic.List<Enemies.EnemyBase> damagedEnemies = new System.Collections.Generic.List<Enemies.EnemyBase>();
        
        int zombiesKilledInThisExplosion = 0; 

        foreach (Collider hit in hits)
        {
            Enemies.EnemyBase enemy = hit.GetComponentInParent<Enemies.EnemyBase>();

            if (enemy != null)
            {
                if (!damagedEnemies.Contains(enemy))
                {
                    damagedEnemies.Add(enemy);
                    enemy.TakeDamage(explosionDamage);
                    if (enemy == null || !enemy.gameObject.activeInHierarchy)
                    {
                        zombiesKilledInThisExplosion++;
                    }
                }
                continue;
            }

            if (damagePlayer)
            {
                PlayerHealth player = hit.GetComponentInParent<PlayerHealth>();
                if (player != null)
                {
                    player.TakeDamage(explosionDamage);
                }
            }
        }

        if (explosionEffectPrefab != null)
        {
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}