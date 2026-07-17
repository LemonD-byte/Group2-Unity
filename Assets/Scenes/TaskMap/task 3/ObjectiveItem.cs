using UnityEngine;

public class ObjectiveItem : MonoBehaviour
{
    [Header("--- CẤU HÌNH TƯƠNG TÁC ---")]
    public float interactionDistance = 3f; 
    private Transform playerTransform;

    void Start()
    {
        FindPlayerAutomatic();
    }

    void Update()
    {
        if (playerTransform == null)
        {
            FindPlayerAutomatic();
            return;
        }

        float distance = Vector3.Distance(transform.position, playerTransform.position);

        if (distance <= interactionDistance)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Collect();
            }
        }
    }

    void FindPlayerAutomatic()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
        else
        {
            GameObject playerByName = GameObject.Find("Player");
            if (playerByName != null)
            {
                playerTransform = playerByName.transform;
            }
        }
    }

    void Collect()
    {
        if (MissionManager3.Instance != null)
        {
            MissionManager3.Instance.CollectMaterial();
        }
        
        Destroy(gameObject); 
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}