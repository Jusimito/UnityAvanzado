using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float maxMovementSpeed = 15.0f;
    [SerializeField] private float acelerationTime = 1f;
    [SerializeField] private AnimationCurve acelerationCurve;
    [SerializeField] private float decelerationTime = 1f;
    [SerializeField] private AnimationCurve decelerationCurve;
    [Space]
    [SerializeField] private float rotationSpeed = 50.0f;
    [SerializeField, Range(0,1)] private float rotationForce = 0.8f;

    Vector2 lastInputValue, inputValue = Vector2.zero;
    float currentSpeed = 0f;
    bool acelerating, decelerating = false;

    public void OnPlayerMove(InputAction.CallbackContext callbackContext)
    {
        lastInputValue = inputValue;
        inputValue = callbackContext.ReadValue<Vector2>();

        if(lastInputValue == Vector2.zero && inputValue != lastInputValue && !acelerating)
        {
            acelerating = true;
            decelerating = false;
            StopAllCoroutines();
            StartCoroutine(Acelerate());
        }

        if(inputValue == Vector2.zero && inputValue != lastInputValue && !decelerating)
        {
            acelerating = false;
            decelerating = true;
            StopAllCoroutines();
            StartCoroutine(Decelerate());
        }

        if(!acelerating && !decelerating)
        {
            currentSpeed = maxMovementSpeed;
        }
    }

    private IEnumerator Acelerate()
    {
        float time = 0;
        float baseSpeed = currentSpeed;
        while (time <= acelerationTime && currentSpeed < maxMovementSpeed)
        {
            Debug.Log("Acelerating: " + currentSpeed);
            currentSpeed = baseSpeed + (maxMovementSpeed * acelerationCurve.Evaluate(time / acelerationTime));
            time += Time.deltaTime;
            yield return null;
        }
        currentSpeed = maxMovementSpeed;
        acelerating = false;
    }

    private IEnumerator Decelerate()
    {
        float time = 0;
        float baseSpeed = currentSpeed;
        while (time <= decelerationTime && currentSpeed > 0)
        {
            Debug.Log("Decelerating: " + currentSpeed);
            currentSpeed = baseSpeed - (maxMovementSpeed * decelerationCurve.Evaluate(time / decelerationTime));
            time += Time.deltaTime;
            yield return null;
        }
        currentSpeed = 0;
        decelerating = false;
    }

    private void Update()
    {        
        transform.rotation = Quaternion.Euler(Vector3.Lerp(transform.rotation.eulerAngles,
            transform.rotation.eulerAngles + Vector3.up * rotationSpeed * inputValue.x * Time.deltaTime, rotationForce));
        transform.Translate(transform.forward * currentSpeed * inputValue.y * Time.deltaTime, Space.World);
    }
}
