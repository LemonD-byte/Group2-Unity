using UnityEngine;

public class CarInteraction2 : MonoBehaviour
{
    [Header("--- CẤU HÌNH TƯƠNG TÁC XE ---")]
    public float interactionDistance = 4f; 
       private Transform playerTransform;

    [Header("--- UI GỢI Ý (TÙY CHỌN) ---")]
    public GameObject interactionHintUI; 

    void Start()
    {
        if (interactionHintUI != null) interactionHintUI.SetActive(false);

        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
        else
        {
            Debug.LogError("CarInteraction: Không tìm thấy đối tượng nào có Tag là 'Player' trong Scene!");
        }
    }

    void Update()
    {
        if (playerTransform == null) return; 

        float distance = Vector3.Distance(transform.position, playerTransform.position);

        if (distance <= interactionDistance)
        {
            if (MissionManager.Instance != null && MissionManager.Instance.IsMissionComplete())
            {
                if (interactionHintUI != null) interactionHintUI.SetActive(true);

                if (Input.GetKeyDown(KeyCode.F))
                {
                    Debug.Log("Bạn đã kích hoạt xe và trốn thoát thành công!");
                    if (interactionHintUI != null) interactionHintUI.SetActive(false);
                    
                    MissionManager.Instance.OnLevelComplete();
                }
            }
            else
            {
                if (interactionHintUI != null) interactionHintUI.SetActive(false);
            }
        }
        else
        {
            if (interactionHintUI != null) interactionHintUI.SetActive(false);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}