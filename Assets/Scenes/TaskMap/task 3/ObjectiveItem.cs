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

        // Không cho nhặt trong lúc đang hiện thoại (tránh phá vỡ thứ tự thoại wave1/wave2 của Task 3)
        if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive) return;

        float distance = Vector3.Distance(transform.position, playerTransform.position);
        if (distance <= interactionDistance)
        {
            if (Input.GetKeyDown(KeyCode.Z))
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
        // Đổi từ MissionManager3 (đã xóa khi gộp) sang MissionManager (class gộp chung)
        if (MissionManager.Instance != null)
        {
            MissionManager.Instance.CollectMaterial();
        }

        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}