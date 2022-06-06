using System.Collections;

using System.Collections.Generic;

using UnityEngine;


public class DragApple : MonoBehaviour
{
    private Vector3 mOffset;

    private float mZCoord;

    //public bool staysAfloat = false;

    // private void OnDestroy()
    // {
    //     EventManager.TriggerEvent("ObjectDestroyed", gameObject);
    // }

    void OnMouseDown()
    {
        mZCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;

        // Store offset = gameobject world pos - mouse world pos
        mOffset = gameObject.transform.position - GetMouseAsWorldPoint();

        // Change tag so that the beaver will recognize the apple only if dragging
        gameObject.tag = "Apple";
        gameObject.GetComponent<Collider>().isTrigger = true;
    }

    void OnFairyDown(GameObject sender)
    {
        gameObject.GetComponent<ChaseWithRigidBody>().target = sender.transform;

    }

    private Vector3 GetMouseAsWorldPoint()
    {
        // Pixel coordinates of mouse (x,y)
        Vector3 mousePoint = Input.mousePosition;

        // z coordinate of game object on screen
        mousePoint.z = mZCoord;

        // Convert it to world points
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    void OnMouseDrag()
    {
        transform.position = GetMouseAsWorldPoint() + mOffset;

        GetComponent<Rigidbody>().isKinematic = false;
    }

    void OnMouseUp()
    {
        gameObject.tag = "AppleUndragged";
        gameObject.GetComponent<Collider>().isTrigger = false;
        
        if (gameObject.transform.position.y < 0)
           gameObject.transform.position = new Vector3(gameObject.transform.position.x, 1.5f, gameObject.transform.position.z);
        if (gameObject.transform.position.x < -7.5f)
           gameObject.transform.position = new Vector3(-7.0f, gameObject.transform.position.y, gameObject.transform.position.z);
        else if (gameObject.transform.position.x > 7.5f)
           gameObject.transform.position = new Vector3(7.0f, gameObject.transform.position.y, gameObject.transform.position.z);

    }

    void OnFairyUp()
    {
        gameObject.GetComponent<ChaseWithRigidBody>().target = null;

    }

    // This is to avoid colliding other apples while dragging one
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Apple")
        {
            GetComponent<Rigidbody>().isKinematic = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        SendMessage("AppleCollision", gameObject, SendMessageOptions.DontRequireReceiver);
    }
}