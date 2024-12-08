using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damageable : MonoBehaviour
{
    [SerializeField] private string projectileTag;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == projectileTag)
        {
            Destroy(gameObject);
        }    
    }
}
