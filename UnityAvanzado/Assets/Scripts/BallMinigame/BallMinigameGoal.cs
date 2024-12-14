using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMinigameGoal : MonoBehaviour
{
    List<GameObject> ballsInside = new List<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Projectile" && !ballsInside.Contains(other.gameObject))
        {
            ballsInside.Add(other.gameObject);

            if(ballsInside.Count >= BallMinigameManager.Instance.BallsCount)
            {
                Debug.Log("YOU WIN!!");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Projectile" && ballsInside.Contains(other.gameObject))
        {
            ballsInside.Remove(other.gameObject);
        }
    }
}
