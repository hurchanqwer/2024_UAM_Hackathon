using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirForce : MonoBehaviour
{
    VectorFieldManager VectorFieldManager;

    public Rigidbody rb;

    private void Start()
    {
        VectorFieldManager = GameObject.Find("VectorField").GetComponent<VectorFieldManager>();
    }
    private void OnTriggerStay(Collider other)

    {
        if (other.CompareTag("Arrow"))
        {
            int index = VectorFieldManager.createdVector.IndexOf(other.gameObject);
            float speed = VectorFieldManager.speeds[index];
            Vector3 forceVector = VectorFieldManager.velocities[index].normalized;
            rb.AddForce(forceVector * speed, ForceMode.Force);
        }
    }

}
