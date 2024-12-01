using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private float minCameraDistance;
    [SerializeField] private float maxCameraDistance;
    CinemachineComposer composer;
    Cinemachine3rdPersonFollow cinemachineFollow;

    private void Start()
    {
        composer = virtualCamera.GetCinemachineComponent<CinemachineComposer>();
        cinemachineFollow = virtualCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
    }

    public void UpdateCameraState(PlayerController playerController)
    {
        cinemachineFollow.CameraDistance = Mathf.Lerp(minCameraDistance, maxCameraDistance, 
            MathExtensions.Remap(playerController.HorizontalVelocity.magnitude, 0, playerController.MaxVelocity, 0, 1));

        composer.m_ScreenX = MathExtensions.Remap(-Mathf.Lerp(playerController.RotationInputValue.x, playerController.LastRotationInputValue.x, playerController.RotationModificationAmount),
            -1, 1, 0.3f, 0.7f);
    }

    public void ShakeCamera() 
    {
        transform.DOShakePosition(0.1f);
    }
}
