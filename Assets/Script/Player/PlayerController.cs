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

    [Header("Weapon Selection")]
    public GameObject meleeObject;
    public GameObject pistolObject;
    public GameObject shotgunObject;
    public GameObject rifleObject;

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

        string chosenWeapon = PlayerPrefs.GetString("SelectedWeapon", "Pistol");

        if (meleeObject != null) meleeObject.SetActive(true); 
        if (pistolObject != null) pistolObject.SetActive(false);
        if (shotgunObject != null) shotgunObject.SetActive(false);
        if (rifleObject != null) rifleObject.SetActive(false);

        switch (chosenWeapon)
        {
            case "Pistol":
                if (pistolObject != null) pistolObject.SetActive(true);
                break;
            case "Shotgun":
                if (shotgunObject != null) shotgunObject.SetActive(true);
                break;
            case "Rifle":
                if (rifleObject != null) rifleObject.SetActive(true);
                break;
            default:
                if (pistolObject != null) pistolObject.SetActive(true);
                break;
        }
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