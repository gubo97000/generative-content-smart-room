using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Add rotation to the glowing circles that appear in the tutorial
public class TutorialGlowingRing : MonoBehaviour
{
    public Vector3 speed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(speed * Time.deltaTime);
    }
}
