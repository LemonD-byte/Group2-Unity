using UnityEngine;

public class GetBox : MonoBehaviour
{
    [Header("Cấu hình nhặt thùng")]
    public float khoangCachNhat = 3.0f; 
    public LayerMask layerVatPham;      

    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main; 
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            ThucHienNhatThung();
        }
    }

    void ThucHienNhatThung()
    {
        Ray ray = mainCam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, khoangCachNhat, layerVatPham))
        {
            if (hit.collider.gameObject.name == "box")
            {
                Debug.Log("Đã nhặt chiếc thùng thành công!");
                Destroy(hit.collider.gameObject);
            }
        }
    }
}