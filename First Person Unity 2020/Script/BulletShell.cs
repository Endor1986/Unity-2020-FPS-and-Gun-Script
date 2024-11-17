using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletShell : MonoBehaviour
{
    public float ejectForce = 3f;
    public float ejectTorque = 1f;

    void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Wähle eine zufällige Richtung, die ungefähr seitwärts ist
            Vector3 forceDirection = transform.right + new Vector3(0, 0.3f, 0);
            rb.AddForce(forceDirection * ejectForce, ForceMode.Impulse);
            rb.AddTorque(Random.insideUnitSphere * ejectTorque, ForceMode.Impulse);
        }
    }
}
