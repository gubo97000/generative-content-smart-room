using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Put on the ball object and add some waypoints
public class RollAlongPath : MonoBehaviour
{
    private GameObject[] points;
    [SerializeField] Rigidbody body;
    [SerializeField] float force = 5f;

    int pointIndex = -1;
    bool movementComplete;
    Vector3 heading;


    void Start()
    {
        GetNextPoint();
    }


    void FixedUpdate()
    {
        MoveBall();
    }


    void MoveBall()
    {
        if (movementComplete == false)
        {
            if (Vector3.Distance(points[pointIndex].transform.position, transform.position) <= 0.2f)
            {
                Debug.Log("Beehive waypoint reached");
                GetNextPoint();
            }
            else if (transform.position.y < 1.7)
            {
                body.velocity = heading * force;
            }
        }
    }


    void GetNextPoint()
    {
        pointIndex++;

        if (pointIndex < points.Length)
        {
            heading = (points[pointIndex].transform.position - transform.position).normalized;
        }
        else
        {
            movementComplete = true;

            // Stop all the things
            body.velocity = body.angularVelocity = Vector3.zero;
        }
    }

    public void SetWaypoints(GameObject[] p)
    {
        points = p;
    }

    // Ignore collision with invisible walls.
    // Because sometimes the beehive happened to spawn behind the front wall, and remained stuck behind it
    void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.tag == "Box")
        {
            Physics.IgnoreCollision(collision.collider, GetComponent<Collider>());
        }

    }

}