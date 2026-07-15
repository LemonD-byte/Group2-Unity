using UnityEngine;

public class Door : MonoBehaviour
{
    [Header("Cấu hình ban đầu")]
    public bool cuaDangMoSan = true; 
    public float gocMoCua = 90f;     
    public float tocDoQuay = 3f;     
    public float khoangCachNhan = 3f; 

    private bool dangMo;
    private Quaternion gocDong;
    private Quaternion gocMo;
    private Transform nguoiChoi;

    void Start()
    {
        
        gocMo = transform.localRotation;
        
     
        gocDong = Quaternion.Euler(0, -gocMoCua, 0) * gocMo;
        
        
        dangMo = true;

        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null) nguoiChoi = playerObj.transform;
    }

    void Update()
    {
        if (nguoiChoi == null || Vector3.Distance(transform.position, nguoiChoi.position) > khoangCachNhan)
            return;

        if (Input.GetKeyDown(KeyCode.F))
        {
            dangMo = !dangMo;
        }

        Quaternion mucTieu = dangMo ? gocMo : gocDong;
        transform.localRotation = Quaternion.Slerp(transform.localRotation, mucTieu, Time.deltaTime * tocDoQuay);
    }
}