using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.W)) { transform.position += transform.forward * 0.3f; }
        if (Input.GetKey(KeyCode.S)) { transform.position += -transform.forward * 0.3f; }
        if (Input.GetKey(KeyCode.A)) { transform.position += -transform.right * 0.3f; }
        if (Input.GetKey(KeyCode.D)) { transform.position += transform.right * 0.3f; }

        if (Input.GetKey(KeyCode.LeftControl)) { transform.position += transform.up * 0.3f; }
        if (Input.GetKey(KeyCode.LeftShift)) { transform.position += -transform.up * 0.3f; }

        if (Input.GetKey(KeyCode.LeftArrow)) { transform.Rotate(0, -1, 0); } 
        if (Input.GetKey(KeyCode.RightArrow)) { transform.Rotate(0, 1, 0); }

    }
}
