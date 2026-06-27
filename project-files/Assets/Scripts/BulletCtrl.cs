using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCtrl : MonoBehaviour
{
    public float damage = 20.0f;

    public float force = 1500.0f;

    private Rigidbody rb;

    void Start() {
        rb = GetComponent<Rigidbody>();

        rb.AddForce(transform.forward * force);
    }
}
