using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Branch : MonoBehaviour
{
    private void OnDestroy()
    {
        Destroy(transform.parent.gameObject);
    }
}
