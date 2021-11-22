using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ChaseWithRigidBody : MonoBehaviour
{
    private Rigidbody rb;
    public Transform target = null;
    public float speed = 1.0f;
    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null) return;
        Vector3 direction = target.position - transform.position;
        // rb.MovePosition(transform.position + direction * Time.deltaTime);
        rb.velocity = direction.normalized * speed;
    }
}