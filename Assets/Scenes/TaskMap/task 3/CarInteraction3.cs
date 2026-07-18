using UnityEngine;

public class CarInteraction3 : MonoBehaviour
{
    [Header("--- CẤU HÌNH TƯƠNG TÁC ---")]
    public float interactionDistance = 4f; 
    private Transform playerTransform;      

    [Header("--- UI GỢI Ý (NẾU CÓ) ---")]
    [Tooltip("Dòng chữ hiện 'Nhấn F để lên xe' (Có thể để trống)")]
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

        float distance = Vector3.Distance(transform.position, playerTransform.position);

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