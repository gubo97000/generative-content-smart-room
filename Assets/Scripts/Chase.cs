using UnityEngine;
using System.Collections;

public class Chase : MonoBehaviour
{
    public int speed;
    public GameObject player;

    void Update()
    {
        Vector3 localPosition = player.transform.position - transform.position;
        localPosition = localPosition.normalized; // The normalized direction in LOCAL space
        //I think there's the need to unpack it:
        // localPosition * Time.deltaTime * speed; //Should do the work
        transform.Translate(localPosition.x * Time.deltaTime * speed, localPosition.y * Time.deltaTime * speed, localPosition.z * Time.deltaTime * speed);
    }
}