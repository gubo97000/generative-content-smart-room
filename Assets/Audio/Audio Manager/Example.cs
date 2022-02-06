using System.Collections;
using UnityEngine;
[RequireComponent(typeof(MeshCollider))]

public class Example : MonoBehaviour
{
  
    private Vector3 screenPoint;
    private Vector3 offset;
 
    void OnMouseDown()
    {
        screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
 
        offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
 
    }
 
    void OnMouseDrag()
    {
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
 
        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
        transform.position = curPosition;
 
    }
    
    void Start(){
        Debug.Log("Wind starts to play");
        FindObjectOfType<AudioManager>().Add(gameObject,"Wind");
    }  
}
