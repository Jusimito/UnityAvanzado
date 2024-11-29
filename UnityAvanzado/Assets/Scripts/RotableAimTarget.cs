using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RotableAimTarget : MonoBehaviour
{
    [SerializeField] private float horizontalRotationSpeed = 5;
    [SerializeField] private float verticalRotationSpeed = 5;
    [SerializeField] private float radius = 5;
    [SerializeField] private float baseHeight = 5;
    [SerializeField] private float horizontalLimit = 85;
    [SerializeField] private float verticalLimit = 0.5f;

    Vector2 rotateValue;
    Vector3 correctedPosition;

    float horizontalDegree;
    float startDegree;

    private void Start()
    {
        horizontalDegree = Vector3.Angle(transform.position - transform.parent.position, Vector3.right) * Mathf.Deg2Rad;
        startDegree = horizontalDegree;
    }

    public void OnPlayerLookAt(InputAction.CallbackContext callbackContext)
    {
        rotateValue = callbackContext.ReadValue<Vector2>();
    }

    void Update()
    {
        horizontalDegree += rotateValue.x * horizontalRotationSpeed * Time.deltaTime;
        if (horizontalDegree < startDegree - horizontalLimit * Mathf.Deg2Rad) horizontalDegree = startDegree - horizontalLimit * Mathf.Deg2Rad;
        if (horizontalDegree > startDegree + horizontalLimit * Mathf.Deg2Rad) horizontalDegree = startDegree + horizontalLimit * Mathf.Deg2Rad;

        correctedPosition = transform.parent.position + new Vector3(radius * Mathf.Cos(horizontalDegree), baseHeight, radius * Mathf.Sin(horizontalDegree));

        correctedPosition.y += rotateValue.y * verticalRotationSpeed * Time.deltaTime;
        if (correctedPosition.y > baseHeight + verticalLimit) correctedPosition.y = baseHeight + verticalLimit;
        if (correctedPosition.y < baseHeight - verticalLimit) correctedPosition.y = baseHeight - verticalLimit;

        transform.localPosition = correctedPosition - transform.parent.position;
    }
}
