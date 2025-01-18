using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField, Range(0,1)] private float snapRatio = 0.2f;
    [SerializeField] private Animator animator;

    float horizontalMovementRaw, verticalMovementRaw;
    float horizontalMovement, verticalMovement;
    // Update is called once per frame
    void Update()
    {
        horizontalMovementRaw = Input.GetAxis("Horizontal");
        verticalMovementRaw = Input.GetAxis("Vertical");

        horizontalMovement = Mathf.Lerp(horizontalMovement, horizontalMovementRaw, snapRatio);
        verticalMovement = Mathf.Lerp(verticalMovement, verticalMovementRaw, snapRatio);

        animator.SetFloat("Speed", verticalMovement);
        animator.SetFloat("Direction", horizontalMovement);
    }
}
