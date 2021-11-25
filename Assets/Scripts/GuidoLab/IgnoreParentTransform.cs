using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnoreParentTransform : MonoBehaviour
{
    public bool rotation = true;
    private Transform _parent;
    // Start is called before the first frame update
    void Start()
    {
        var _parent = transform.parent;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.rotation = Quaternion.Euler(transform.parent.rotation.eulerAngles * -1);
    }
}
