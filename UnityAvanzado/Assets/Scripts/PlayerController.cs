using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using Cinemachine;

public enum MovementState
{
    Accelerating,
    Decelerating,
    MaxAcceleration
}

public class PlayerController : MonoBehaviour
{
    [SerializeField] private CharacterController controller;
    [SerializeField] private Transform characterMesh;
    [SerializeField] private CameraController cameraController;
    [Space]
    [SerializeField] private float maxVelocity = 10.0f;
    [SerializeField] private float rotationSpeed = 0.1f;
    [SerializeField, Range(0, 1)] private float rotationModificationAmount = 0.5f;
    [SerializeField, Range(0,1)] private float inertiaModificationValue = 0.5f;
    [SerializeField] private float maxAceleration = 1f;
    [SerializeField] private float acelerationTime = 1f;
    [SerializeField] private AnimationCurve acelerationCurve;
    [SerializeField] private float maxDeceleration = 3f;
    [SerializeField] private float decelerationTime = 1f;
    [SerializeField] private AnimationCurve decelerationCurve;
    [Space]
    [SerializeField] private float jumpForce = 5.0f;
    [SerializeField] private float gravityMultiplier = 2.3f;

    private float controlTime = 0f;
    private MovementState movementState = MovementState.Decelerating;

    private float currentAcceleration;
    private Vector2 horizontalVelocity;
    private float yVelocity;
    private Vector2 movementInputValue;
    private Vector2 lastRotationInputValue;
    private Vector2 rotationInputValue;
    private bool isGrounded = true;
    private bool jumpPressed = false;

    Sequence landAnimation;

    public Vector2 MovementInputValue => movementInputValue;
    public Vector2 RotationInputValue => rotationInputValue;
    public Vector2 LastRotationInputValue => lastRotationInputValue;
    public float RotationModificationAmount => rotationModificationAmount;
    public Vector2 HorizontalVelocity => horizontalVelocity;
    public float MaxVelocity => maxVelocity;

    public void OnPlayerMove(InputAction.CallbackContext callbackContext)
    {
        movementInputValue = callbackContext.ReadValue<Vector2>();
    }

    public void OnPlayerRotate(InputAction.CallbackContext callbackContext)
    {
        lastRotationInputValue = rotationInputValue;
        rotationInputValue = callbackContext.ReadValue<Vector2>();
    }

    public void OnJumpPressed(InputAction.CallbackContext callbackContext)
    {
        bool lastJumpPressed = jumpPressed;
        jumpPressed = callbackContext.ReadValue<float>() > 0;
        if (!lastJumpPressed && jumpPressed && isGrounded)
        {
            Sequence jumpAnimation = DOTween.Sequence();
            jumpAnimation.Append(characterMesh.transform.DOScaleY(0.6f, 0.05f).SetEase(Ease.InFlash));
            jumpAnimation.AppendCallback(() => yVelocity += jumpForce);
            jumpAnimation.Append(characterMesh.transform.DOScaleY(1.0f, 0.1f).SetEase(Ease.OutSine));
            jumpAnimation.Play();
        }

    }

    // Update is called once per frame
    void Update()
    {
        CheckPlayerStatus();
        RotatePlayer();
        CheckAccelerationValue();
        ProcessInput();

        cameraController.UpdateCameraState(this);
    }

    private void LateUpdate()
    {
        Vector3 movementAmount = (transform.forward * horizontalVelocity.y * Time.deltaTime) + (transform.up * yVelocity * Time.deltaTime) + (transform.right * horizontalVelocity.x * Time.deltaTime);
        controller.Move(movementAmount);
    }

    private void CheckPlayerStatus()
    {
        bool lastGrounded = isGrounded;
        isGrounded = Physics.CheckSphere(transform.position + Vector3.up * controller.radius * 0.5f, controller.radius, 1 << 8);
        if (!lastGrounded && isGrounded)
        {
            landAnimation = DOTween.Sequence();
            landAnimation.Append(characterMesh.transform.DOScaleY(1.2f, 0.05f).SetEase(Ease.InFlash));
            landAnimation.Append(characterMesh.transform.DOScaleY(0.6f, 0.1f).SetEase(Ease.InFlash));
            landAnimation.Append(characterMesh.transform.DOScaleY(1.0f, 0.1f).SetEase(Ease.OutSine));
            landAnimation.Play();

            if (yVelocity <= -10)
            {
                cameraController.ShakeCamera();
            }
        }
        if (isGrounded)
        {
            yVelocity = 0f;
        }
    }

    private void RotatePlayer()
    {
        Vector3 currentRotation = transform.rotation.eulerAngles;
        Vector3 nextRotation = currentRotation + (Vector3.up * rotationInputValue.x * rotationSpeed);
        transform.rotation = Quaternion.Euler(Vector3.Lerp(currentRotation, nextRotation, rotationModificationAmount));
    }

    private void CheckAccelerationValue()
    {
        if (movementInputValue != Vector2.zero)
        {
            if(movementState == MovementState.Decelerating)
            {
                movementState = MovementState.Accelerating;
                controlTime = Time.time;
            }
            if(movementState == MovementState.Accelerating)
            {
                currentAcceleration = maxAceleration * acelerationCurve.Evaluate((Time.time - controlTime) / acelerationTime);
                if (currentAcceleration >= maxAceleration) movementState = MovementState.MaxAcceleration;
            }
        }
        else
        {
            if (movementState != MovementState.Decelerating)
            {
                movementState = MovementState.Decelerating;
                controlTime = Time.time;
            }
            if (movementState == MovementState.Decelerating)
            {
                currentAcceleration = maxDeceleration * decelerationCurve.Evaluate((Time.time - controlTime) / decelerationTime);
            }
        }
    }

    private void ProcessInput()
    {
        if (movementInputValue != Vector2.zero)
        {
            if(Mathf.Sign(horizontalVelocity.x) != Mathf.Sign(movementInputValue.x))
            {
                horizontalVelocity.x *= inertiaModificationValue;
            }
            if (Mathf.Sign(horizontalVelocity.y) != Mathf.Sign(movementInputValue.y))
            {
                horizontalVelocity.y *= inertiaModificationValue;
            }
            horizontalVelocity += movementInputValue * currentAcceleration * Time.deltaTime;
        }
        else
        {
            horizontalVelocity += currentAcceleration * horizontalVelocity * Time.deltaTime;
        }

        horizontalVelocity.x = Mathf.Max(horizontalVelocity.x, -maxVelocity);
        horizontalVelocity.x = Mathf.Min(horizontalVelocity.x, maxVelocity);
        horizontalVelocity.y = Mathf.Max(horizontalVelocity.y, -maxVelocity);
        horizontalVelocity.y = Mathf.Min(horizontalVelocity.y, maxVelocity);

        if (!isGrounded)
        {
            yVelocity += gravityMultiplier * Physics.gravity.y * Time.deltaTime;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position + Vector3.up * controller.radius * 0.5f, controller.radius);
    }
}
