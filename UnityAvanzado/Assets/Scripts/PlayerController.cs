using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private CharacterController controller;
    [SerializeField] private float maxVelocity = 10.0f;
    [SerializeField] private float acceleration = 1f;
    [SerializeField] private float deceleration = 3f;
    [SerializeField] private float jumpForce = 5.0f;

    private float velocity;
    private float yVelocity;
    private Vector2 inputValue;
    private bool isGrounded = true;
    private bool jumpPressed = false;

    public void OnPlayerMove(InputAction.CallbackContext callbackContext)
    {
        inputValue = callbackContext.ReadValue<Vector2>();
    }

    public void OnJumpPressed(InputAction.CallbackContext callbackContext)
    {
        jumpPressed = callbackContext.ReadValue<float>() > 0;
    }

    // Update is called once per frame
    void Update()
    {
        CheckPlayerStatus();
        RotatePlayer();
        ProcessInput();

        controller.Move(transform.forward * velocity * Time.deltaTime);
        controller.Move(transform.up * yVelocity * Time.deltaTime);
    }

    private void CheckPlayerStatus()
    {
        isGrounded = controller.isGrounded || transform.position.y <= controller.skinWidth;
        if (isGrounded)
        {
            yVelocity = 0f;
        }
    }

    private void RotatePlayer()
    {
        if (inputValue != Vector2.zero)
        {
            gameObject.transform.forward = new Vector3(inputValue.x, 0, inputValue.y);
        }
    }

    private void ProcessInput()
    {
        if (inputValue != Vector2.zero)
        {
            velocity += inputValue.magnitude * acceleration * Time.deltaTime;
        }
        else
        {
            velocity -= deceleration * velocity * Time.deltaTime;
        }
        
        velocity = Mathf.Max(velocity, 0);
        velocity = Mathf.Min(velocity, maxVelocity);
        
        // Makes the player jump
        if (jumpPressed && isGrounded)
        {
            yVelocity += jumpForce;
        }
        else if (!isGrounded)
        {
            yVelocity += Physics.gravity.y * Time.deltaTime;
        }
    }
}
