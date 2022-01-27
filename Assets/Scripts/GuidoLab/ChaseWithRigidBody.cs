using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ChaseWithRigidBody : MonoBehaviour
{
    private Rigidbody rb;
    public Transform target = null;
    public float speed = 1.0f;
    public float minDistance = 0f;
    public bool inertiaWhenInMinDistance = true;

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
        // direction.y = 0;    // Added this line so it can't float upwards

        // rb.MovePosition(transform.position + direction * Time.deltaTime);

        if(direction.magnitude > minDistance)
            rb.velocity = direction.normalized * speed;
        else if(!inertiaWhenInMinDistance) rb.velocity = Vector3.zero;
    }
}