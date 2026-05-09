using UnityEngine;

public class PlayerMovement2D : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector2 movement;

    [Header("Movement")]
    public bool canMove = true;

    public float normalSpeed = 5f;
    private float currentSpeed;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentSpeed = normalSpeed;
    }

    void Update()
    {
        if (GameManager.InputLocked) return;

        if (!canMove)
        {
            movement = Vector2.zero;
            return;
        }

        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
    }

    void FixedUpdate()
    {
        if (!canMove)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        rb.velocity = movement.normalized * currentSpeed;
    }

    public void ApplySlow(float slowMultiplier)
    {
        currentSpeed = normalSpeed * slowMultiplier;
    }

    public void ResetSpeed()
    {
        currentSpeed = normalSpeed;
    }
}