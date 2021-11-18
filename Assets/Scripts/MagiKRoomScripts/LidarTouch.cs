using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LidarTouch : MonoBehaviour
{

    public Camera camera;
    public LidarTouchPoints point;

    public event Action<LidarTouch, GameObject> TouchedElement; 

    // Start is called before the first frame update
    void Start()
    {
        camera = GameObject.FindObjectsOfType<Camera>().Where(x => x.targetDisplay == MagicRoomManager.instance.systemConfiguration.frontalScreen).FirstOrDefault();

    }

    // Update is called once per frame
    void Update()
    {
        transform.localPosition = new Vector3(point.x * Screen.width - Screen.width / 2, point.y * Screen.height - Screen.height / 2, 0f);
        
        RaycastHit hit;
        Ray ray = new Ray(transform.position, Vector3.forward);

        if (Physics.Raycast(ray, out hit, 100f))
        {
            TouchedElement?.Invoke(this, hit.transform.gameObject);
        }

        ray = new Ray(transform.position, -Vector3.forward);

        if (Physics.Raycast(ray, out hit, 100f))
        {
            TouchedElement?.Invoke(this, hit.transform.gameObject);
        }

        Collider2D[] coll = Physics2D.OverlapCircleAll(camera.WorldToScreenPoint(transform.position), 10);
        if (coll!= null)
        {
            foreach (Collider2D c in coll)
            {
                TouchedElement?.Invoke(this, c.gameObject);
            }
        }

    }
    
    
}
