using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [Header("Mouse Settings")]
    public float mouseSensitivity;

    [Header("Player")]
    public Transform playerBody;

    private float xRotation = 0f;

    void Start()
    {
        // Khóa chuột vào giữa màn hình
        Cursor.lockState = CursorLockMode.Locked;

        // Ẩn con trỏ chuột
        Cursor.visible = false;
    }

    void Update()
    {
        // Lấy chuyển động của chuột
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Xoay camera lên xuống
        xRotation -= mouseY;

        // Giới hạn góc nhìn lên xuống
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Xoay camera theo trục X
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Xoay Player theo trục Y (trái/phải)
        playerBody.Rotate(Vector3.up * mouseX);
    }
}