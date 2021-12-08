using UnityEngine;

public class FollowThePath : MonoBehaviour
{

    // Array of waypoints to walk from one to the next one
    [SerializeField]
    public Transform[] waypoints;
    
    public bool loop;
    // Walk speed that can be set in Inspector
    [SerializeField]
    private float moveSpeed = 2f;

    // Index of current waypoint from which object walks
    // to the next one
    private int waypointIndex = 0;

    // The animator to manage when the object has completed the path
    private Animator anim;

    // Use this for initialization
    private void Start()
    {

        // Set position of object as position of the first waypoint
        transform.position = waypoints[waypointIndex].transform.position;

        // Get the animator for managing the state later on
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    private void Update()
    {
        Move();
    }

    // Method that actually make object walk
    private void Move()
    {
        // If object didn't reach last waypoint it can move
        // If object reached last waypoint then it stops
        if (waypointIndex <= waypoints.Length - 1)
        {
            // The animator here will know that it needs the run animation
            // anim.SetBool("hasReachedSpot", false);

            // Move object from current waypoint to the next one
            // using MoveTowards method
            transform.position = Vector3.MoveTowards(transform.position,
               waypoints[waypointIndex].transform.position,
               moveSpeed * Time.deltaTime);


            // Rotate to face the next waypoint
            Vector3 targetDirection = waypoints[waypointIndex].transform.position - transform.position;

            Vector3 newDirection = Vector3.RotateTowards(transform.forward,
               targetDirection,
               moveSpeed * Time.deltaTime * 10, 0.0f);

            transform.rotation = Quaternion.LookRotation(newDirection);

            // If object reaches position of waypoint he walked towards
            // then waypointIndex is increased by 1
            // and object starts to walk to the next waypoint
            if (transform.position == waypoints[waypointIndex].transform.position)
            {
                waypointIndex += 1;
            }
        }
        else if (loop)
        {
            waypointIndex = 0;
        }
        else
        {
            // The animator stops running
            // anim.SetBool("hasReachedSpot", true);
        }
    }
}