using UnityEngine;

public class PlayerMovement2D : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector2 input;
    private Vector2 currentVelocity;

    [Header("Movement")]
    public bool canMove = true;

    public float normalSpeed = 5f;
    private float currentSpeed;

    [Header("Feel")]
    public float acceleration = 18f;
    public float deceleration = 24f;
    public float turnAcceleration = 30f;
    [Header("Powerups")]
public bool hasParkBoots = false;
public float parkBootSpeedMultiplier = 1.6f;

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
            input = Vector2.zero;
            return;
        }

        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");

        input = input.normalized;
    }

    void FixedUpdate()
    {
        if (!canMove)
        {
            rb.velocity = Vector2.Lerp(
                rb.velocity,
                Vector2.zero,
                deceleration * Time.fixedDeltaTime
            );
            return;
        }

        Vector2 targetVelocity = input * currentSpeed;

        // changing direction sharply = stronger acceleration
        float accel = acceleration;

        if (rb.velocity.magnitude > 0.1f &&
            Vector2.Dot(rb.velocity.normalized, targetVelocity.normalized) < 0f)
        {
            accel = turnAcceleration;
        }

        // no input = decelerate smoothly
        if (input == Vector2.zero)
        {
            rb.velocity = Vector2.Lerp(
                rb.velocity,
                Vector2.zero,
                deceleration * Time.fixedDeltaTime
            );
        }
        else
        {
            rb.velocity = Vector2.Lerp(
                rb.velocity,
                targetVelocity,
                accel * Time.fixedDeltaTime
            );
        }
    }

    public void ApplySlow(float slowMultiplier)
{
    currentSpeed = normalSpeed * slowMultiplier;
}

public void ResetSpeed()
{
    if (hasParkBoots)
        currentSpeed = normalSpeed * parkBootSpeedMultiplier;
    else
        currentSpeed = normalSpeed;
}

public void EquipParkBoots()
{
    hasParkBoots = true;
    currentSpeed = normalSpeed * parkBootSpeedMultiplier;

    Debug.Log("ParkBoots equipped");
}

public void BreakParkBoots()
{
    hasParkBoots = false;
    currentSpeed = normalSpeed;

    Debug.Log("ParkBoots broke");
}
}