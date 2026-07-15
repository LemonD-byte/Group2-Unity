using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 15f;

    [Header("Dash")]
    public float dashPower = 50f;
    public float dashTime = 0.2f;
    public float dashCooldown = 0.75f;

    private bool canDash = true;
    private bool isDashing = false;

    private float horizontal;
    private float vertical;

    private Rigidbody rb;
    private Animator animator; 

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        animator = GetComponent<Animator>(); 
    }

    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        }

        UpdateAnimator();
    }

    void FixedUpdate()
    {
        if (!isDashing)
        {
            Move();
        }
    }

    void Move()
    {
        Vector3 move = (transform.right * horizontal + transform.forward * vertical).normalized;
        rb.MovePosition(rb.position + move * speed * Time.fixedDeltaTime);
    }

    void UpdateAnimator()
    {
        if (animator == null) return;

        // Tính độ lớn di chuyển (0 = đứng yên, 1 = full tốc độ)
        float moveMagnitude = new Vector2(horizontal, vertical).magnitude;
        animator.SetFloat("Speed", moveMagnitude);
    }

    IEnumerator Dash()
    {
        if (horizontal == 0 && vertical == 0)
            yield break;

        canDash = false;
        isDashing = true;

        Vector3 dashDirection =
            (transform.right * horizontal + transform.forward * vertical).normalized;

        float timer = 0f;
        while (timer < dashTime)
        {
            rb.MovePosition(
                rb.position +
                dashDirection * dashPower * Time.fixedDeltaTime);
            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        isDashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
}