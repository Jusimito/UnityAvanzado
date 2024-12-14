 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMinigameManager : MonoBehaviour
{
    private static BallMinigameManager instance;
    public static BallMinigameManager Instance => instance;

    [SerializeField] private float force = 1000;
    [SerializeField] private float forceAngle = 45;
    [SerializeField] private Transform leftEmitter;
    [SerializeField] private Transform rightEmitter;

    List<Rigidbody> balls = new List<Rigidbody>();

    public int BallsCount => balls.Count;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        GameObject[] ballsGO = GameObject.FindGameObjectsWithTag("Projectile");
        foreach (GameObject ballGO in ballsGO)
        {
            balls.Add(ballGO.GetComponent<Rigidbody>());
        }
    }

    public void ApplyLeftForce()
    {
        foreach (var b in balls)
        {
            b.AddForceAtPosition(new Vector3(Mathf.Cos(-forceAngle * Mathf.Deg2Rad), Mathf.Sin(forceAngle * Mathf.Deg2Rad), 0) * force, leftEmitter.position);
        }
    }

    public void ApplyRightForce()
    {
        foreach (var b in balls)
        {
            b.AddForceAtPosition(new Vector3(Mathf.Cos((90 + forceAngle) * Mathf.Deg2Rad), Mathf.Sin(forceAngle * Mathf.Deg2Rad), 0) * force, rightEmitter.position);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(leftEmitter.position, leftEmitter.position + new Vector3(Mathf.Cos(-forceAngle * Mathf.Deg2Rad), Mathf.Sin(forceAngle * Mathf.Deg2Rad), 0) * force);
        Gizmos.DrawLine(rightEmitter.position, rightEmitter.position + new Vector3(Mathf.Cos((90+forceAngle) * Mathf.Deg2Rad), Mathf.Sin(forceAngle * Mathf.Deg2Rad), 0) * force);
    }
}
