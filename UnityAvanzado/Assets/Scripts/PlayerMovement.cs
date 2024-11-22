using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float maxMovementSpeed = 15.0f;
    [SerializeField] private float rotationSpeed = 50.0f;
    [SerializeField] private float acelerationTime = 1f;
    [SerializeField] private AnimationCurve acelerationCurve;
    [SerializeField] private float decelerationTime = 1f;
    [SerializeField] private AnimationCurve decelerationCurve;

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
            StartCoroutine(Acelerate());
        }

        if(inputValue == Vector2.zero && inputValue != lastInputValue && !decelerating)
        {
            acelerating = false;
            decelerating = true;
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
        while (time <= acelerationTime)
        {
            Debug.Log("Acelerating: " + currentSpeed);
            currentSpeed = maxMovementSpeed * acelerationCurve.Evaluate(time / acelerationTime);
            time += Time.deltaTime;
            yield return null;
        }

        acelerating = false;
    }

    private IEnumerator Decelerate()
    {
        float time = 0;
        while (time <= decelerationTime)
        {
            Debug.Log("Decelerating: " + currentSpeed);
            currentSpeed = maxMovementSpeed * decelerationCurve.Evaluate(time / decelerationTime);
            time += Time.deltaTime;
            yield return null;
        }

        decelerating = false;
    }

    private void Update()
    {
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + Vector3.up * rotationSpeed * inputValue.x * Time.deltaTime);
        transform.Translate(transform.forward * currentSpeed * inputValue.y * Time.deltaTime, Space.World);
    }
}
