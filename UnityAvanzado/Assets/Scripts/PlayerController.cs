using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

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
    [SerializeField] private float maxVelocity = 10.0f;
    [SerializeField] private float maxAceleration = 1f;
    [SerializeField] private AnimationCurve acelerationCurve;
    [SerializeField] private float maxDeceleration = 3f;
    [SerializeField] private AnimationCurve decelerationCurve;
    [Space]
    [SerializeField] private float jumpForce = 5.0f;
    [SerializeField] private float gravityMultiplier = 2.3f;

    private float controlTime = 0f;
    private MovementState movementState = MovementState.Decelerating;

    private float currentAcceleration;
    private float velocity;
    private float yVelocity;
    private Vector2 inputValue;
    private bool isGrounded = true;
    private bool jumpPressed = false;

    Sequence landAnimation;

    private void Start()
    {
        landAnimation = DOTween.Sequence();
        landAnimation.SetAutoKill(false);
        landAnimation.Append(characterMesh.transform.DOScaleY(1.2f, 0.05f).SetEase(Ease.InFlash));
        landAnimation.Append(characterMesh.transform.DOScaleY(0.6f, 0.1f).SetEase(Ease.InFlash));
        landAnimation.Append(characterMesh.transform.DOScaleY(1.0f, 0.1f).SetEase(Ease.OutSine));
    }
    public void OnPlayerMove(InputAction.CallbackContext callbackContext)
    {
        inputValue = callbackContext.ReadValue<Vector2>();
    }

    public void OnJumpPressed(InputAction.CallbackContext callbackContext)
    {
        bool lastJumpPressed = jumpPressed;
        jumpPressed = callbackContext.ReadValue<float>() > 0;
        if (!lastJumpPressed && jumpPressed)
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
    }

    private void LateUpdate()
    {
        Vector3 movementAmount = (transform.forward * velocity * Time.deltaTime) + (transform.up * yVelocity * Time.deltaTime);
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
        }
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

    private void CheckAccelerationValue()
    {
        if (inputValue != Vector2.zero)
        {
            if(movementState == MovementState.Decelerating)
            {
                movementState = MovementState.Accelerating;
                controlTime = Time.time;
            }
            if(movementState == MovementState.Accelerating)
            {
                currentAcceleration = maxAceleration * acelerationCurve.Evaluate(Time.time - controlTime);
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
                currentAcceleration = maxDeceleration * decelerationCurve.Evaluate(Time.time - controlTime);
            }
        }
    }

    private void ProcessInput()
    {
        if (inputValue != Vector2.zero)
        {
            velocity += inputValue.magnitude * currentAcceleration * Time.deltaTime;
        }
        else
        {
            velocity += currentAcceleration * velocity * Time.deltaTime;
        }
        
        velocity = Mathf.Max(velocity, 0);
        velocity = Mathf.Min(velocity, maxVelocity);
        
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
