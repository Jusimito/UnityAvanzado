using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform targetToFollow;

    private Vector3 lastTargetPosition;
    private Vector3 targetDelta;

    private void Start()
    {
        lastTargetPosition = targetToFollow.position;
    }

    // Update is called once per frame
    void Update()
    {
        targetDelta = targetToFollow.position - lastTargetPosition;
        lastTargetPosition = targetToFollow.position;

        transform.position += targetDelta;    
    }
}
