using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(Mathf.Sin(Time.timeSinceLevelLoad), 0,0);

        transform.rotation = Quaternion.identity;
        transform.Rotate(0,360 * Mathf.Sin(Time.timeSinceLevelLoad), 0, Space.World);

        Vector3 scale = transform.localScale;
        scale.y = Mathf.Max(Mathf.Abs(Mathf.Cos(Time.timeSinceLevelLoad)), 0.5f);
        transform.localScale = scale;
    }
}
