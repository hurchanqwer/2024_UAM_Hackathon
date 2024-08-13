using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FanAnim : MonoBehaviour
{
    public Transform L, R;
    public Rigidbody rb;
    private void FixedUpdate()
    {
        L.Rotate(Vector3.up, 5f);
        R.Rotate(Vector3.up, -5f);

    }
}
