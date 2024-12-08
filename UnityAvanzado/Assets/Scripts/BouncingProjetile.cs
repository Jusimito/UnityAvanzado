using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncingProjetile : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    [SerializeField] float startForce = 2000;

    // Start is called before the first frame update
    void Start()
    {
        
        Vector3 randomStartVector = new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1)).normalized;

        rb.AddForce(randomStartVector * startForce);
    }
}
