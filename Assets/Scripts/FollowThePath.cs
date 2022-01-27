using System.Linq;
using UnityEngine;

public class FollowThePath : MonoBehaviour
{

    // Array of waypoints to walk from one to the next one
    [SerializeField]
    public Transform[] waypoints;
    
    public bool loop;
    public bool isFish;
    public bool isBee;
    // Walk speed that can be set in Inspector
    [SerializeField]
    private float moveSpeed = 2f;

    // Index of current waypoint from which object walks
    // to the next one
    private int waypointIndex = 0;

    private bool hasCompletedPath = false;

    private int progressiveNumber;

    // Use this for initialization
    private void Start()
    {
        if (isFish) {
            
            var fishPath = GameObject.Find("Fish path");
            var i = 0;
            Debug.Log(fishPath);
            
          
            foreach (Transform waypoint in fishPath.transform)
            {
                waypoints[i] = waypoint;
                i++;
            }
            
        }
        else if (isBee)
        {
            var beePath = GameObject.Find("Bee Path");
            var i = 1;
            Debug.Log(beePath);

            waypoints[0] = this.transform;
            foreach (Transform waypoint in beePath.transform)
            {
                waypoints[i] = waypoint;
                i++;
            }
        }

        // Set position of object as position of the first waypoint
        transform.position = waypoints[waypointIndex].transform.position;

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
        else if (loop)  // Restart the loop
        {
            waypointIndex = 0;
        }
        else if (!hasCompletedPath)    // End of path
        {
            // If there is an animator, this event can be handled to change state
            EventManager.TriggerEvent("EndOfPath", gameObject);
            Debug.Log(gameObject);

            // Set hasCompletedPath to true, to avoid firing events continuously
            hasCompletedPath = true;
        }
    }

    public void ResetPath(GameObject[] newPaths)
    {
        EventManager.TriggerEvent("ResetPath", gameObject);

        // newPaths is an array of paths. But the actual waypoints are children of each "path".
        // So, retrieve the path, and then its children, in particular their transform
        // Skip 1 beacuse GetComponentsInChildren also returns the father
        waypoints = newPaths[progressiveNumber].GetComponentsInChildren<Transform>().Skip(1).ToArray();
        hasCompletedPath = false;
        waypointIndex = 0;
    }

    // Used for beavers. So to identify the path to take after they all have eaten an apple (to go to the dam)
    public void setProgressiveNumber(int n)
    {
        progressiveNumber = n;
    }
}