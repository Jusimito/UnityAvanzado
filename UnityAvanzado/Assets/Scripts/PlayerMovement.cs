using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 15.0f;
    [SerializeField] private float rotationSpeed = 50.0f;

    float verticalInputAmount;
    float horizontalInputAmount;

    public void OnPlayerMove(InputAction.CallbackContext callbackContext)
    {
        verticalInputAmount = callbackContext.ReadValue<Vector2>().y;
        horizontalInputAmount = callbackContext.ReadValue<Vector2>().x;
    }

    private void Update()
    {
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + Vector3.up * rotationSpeed * horizontalInputAmount * Time.deltaTime);
        transform.Translate(transform.forward * movementSpeed * verticalInputAmount * Time.deltaTime, Space.World);
    }
}
