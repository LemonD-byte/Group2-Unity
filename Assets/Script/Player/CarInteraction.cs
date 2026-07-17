using UnityEngine;

public class CarInteraction : MonoBehaviour
{
    [Header("--- CẤU HÌNH TƯƠNG TÁC ---")]
    public float interactionDistance = 4f;
    private Transform playerTransform;      

    [Header("--- UI GỢI Ý (NẾU CÓ) ---")]
    [Tooltip("Dòng chữ ẩn/hiển thị 'Nhấn F để lên xe' (Có thể để trống)")]
    public GameObject interactPromptUI; 

    void Start()
    {
        FindPlayerAutomatic();

        if (interactPromptUI != null)
        {
            interactPromptUI.SetActive(false);
        }
    }

    void Update()
    {
        if (playerTransform == null)
        {
            FindPlayerAutomatic();
            return; 
        }

        if (MissionManager.Instance != null && !MissionManager.Instance.IsMissionComplete())
        {
            if (interactPromptUI != null && interactPromptUI.activeSelf)
            {
                interactPromptUI.SetActive(false);
            }
            return; 
        }

        // 1. Tính khoảng cách đường thẳng giữa Player và Chiếc Xe
        float distance = Vector3.Distance(transform.position, playerTransform.position);

        // 2. Xử lý hiển thị dòng chữ gợi ý "Nhấn F..." khi đứng đủ gần
        if (distance <= interactionDistance)
        {
            if (interactPromptUI != null && !interactPromptUI.activeSelf)
            {
                interactPromptUI.SetActive(true);
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                TriggerLevelComplete();
            }
        }
        else
        {

            if (interactPromptUI != null && interactPromptUI.activeSelf)
            {
                interactPromptUI.SetActive(false);
            }
        }
    }

    // Hàm phụ trách tự động tìm kiếm Player
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

    void TriggerLevelComplete()
    {
        if (MissionManager.Instance != null)
        {
            MissionManager.Instance.OnLevelComplete();
        }

        if (interactPromptUI != null) interactPromptUI.SetActive(false);
        enabled = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}