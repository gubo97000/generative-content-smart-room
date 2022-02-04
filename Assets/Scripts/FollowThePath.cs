using System.Linq;
using UnityEngine;

public class FollowThePath : MonoBehaviour
{

    // Array of waypoints to walk from one to the next one
    [SerializeField]
    public Transform[] waypoints;
    public int length = 0;

    private bool isEnabled = true;

    // True if spawned object is teleported to first waypoint
    // False if we want it to start moving from where it actually spawned
    public bool startFromWaypoint = true;

    public bool loop;
    public bool rotate = true;

    public string pathName;
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
        GameObject path = GameObject.Find(pathName);

        int i = startFromWaypoint ? 0 : 1;

        if(waypoints == null || waypoints.Length == 0 || System.Array.TrueForAll(waypoints, w => w == null))
        {
            waypoints = new Transform[length];
        }

        if(!startFromWaypoint)
            waypoints[0] = this.transform;

        if (path != null)
        {
            Debug.Log(path);

            foreach (Transform waypoint in path.transform)
            {
                if (waypoint.gameObject.active)
                {
                    waypoints[i] = waypoint;
                    i++;
                }
            }
        }

        // Set position of object as position of the first waypoint
        transform.position = waypoints[waypointIndex].transform.position;

    }

    // Update is called once per frame
    private void Update()
    {
        if(isEnabled)
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

            if (rotate)
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
            EventManager.TriggerEvent("EndOfPath", gameObject, new EventDict() { { "activator", gameObject } });
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


    public void triggerEnabled()
    {
        isEnabled = !isEnabled;
    }
}