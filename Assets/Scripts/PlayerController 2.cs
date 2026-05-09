using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // --- Settings adjustable in the Unity Inspector ---
    public float maxSpeed = 10f;
    public float acceleration = 5f;
    public float deceleration = 3f;
    public float turnSpeed = 100f;

    // --- Private internal variables ---
    private Rigidbody2D _rb;       // Reference to the physics component
    private float _moveInput;      // Stores W/S or Up/Down input (-1 to 1)
    private float _turnInput;      // Stores A/D or Left/Right input (-1 to 1)
    private float _currentSpeed = 0f; // The actual speed the vehicle is moving
    
    // Start is called when the game begins
    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        
        // Prevents the vehicle from "falling" since this is a top-down game
        _rb.gravityScale = 0;

        // Helps the camera follow the physics object smoothly
        _rb.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    // Update is called once per frame (good for capturing keystrokes)
    private void Update() 
    {
        // GetInput axes: Vertical is W/S, Horizontal is A/D
        _moveInput = Input.GetAxisRaw("Vertical");
        _turnInput = Input.GetAxisRaw("Horizontal");
    }
    
    // FixedUpdate is called at a constant rate (best for physics)
    private void FixedUpdate()
    {
        HandleMovement();
        HandleRotation();
    }

    private void HandleMovement() {
        // Check if the player is pressing a move key
        // Gradually increase speed towards maxSpeed in the direction of input
        _currentSpeed = Mathf.Abs(_moveInput) > 0.01f ? Mathf.MoveTowards(_currentSpeed, _moveInput * maxSpeed, acceleration * Time.fixedDeltaTime) : 
            // Gradually slow down to zero when no keys are pressed
            Mathf.MoveTowards(_currentSpeed, 0, deceleration * Time.fixedDeltaTime);

        // Apply the calculated speed to the Rigidbody velocity
        // transform.up is used because 'forward' in 2D is usually the Y-axis
        _rb.velocity = transform.up * _currentSpeed;
    }

    private void HandleRotation()
    {
        // This calculates how much the car should be allowed to turn.
        // It prevents the car from spinning like a top while standing perfectly still.
        float speedFactor = Mathf.Clamp01(_rb.velocity.magnitude / (maxSpeed * 0.1f));

        // Calculate rotation: 
        // -_turnInput is used so that 'D' rotates right (clockwise)
        float rotation = -_turnInput * turnSpeed * speedFactor * Time.fixedDeltaTime;

        // Apply the rotation through the physics system
        _rb.MoveRotation(_rb.rotation + rotation);
    }
}
