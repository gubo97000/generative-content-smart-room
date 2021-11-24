using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HasFloatingInventory : MonoBehaviour
{

    //     [Range(0, 360)]
    //     public float StartAngle = 0;

    //     public bool UseTargetCoordinateSystem = false;

    //     public bool LookAtTarget = false;

    //     private void Awake()
    //     {
    //         // angle = StartAngle;
    //     }

    public float RotationSpeed = 1;

    public float CircleRadius = 3;

    public float ElevationOffset = 0;

    private Vector3 positionOffset;
    private float angle;

    public List<Transform> slots;

    GameObject slotObject;

    private void Start()
    {
        slotObject = new GameObject("FloatingSlot");
        EventManager.StartListening("InventoryAddEvent", addSlot);
        EventManager.StartListening("InventoryRemoveEvent", removeSlot);
    }
    private void OnDestroy()     {
        EventManager.StopListening("InventoryAddEvent", addSlot);
        EventManager.StopListening("InventoryRemoveEvent", removeSlot);
    }


    private void addSlot(EventDict data)
    {
        if (((GameObject)data["receiver"]) == gameObject)
        {
            GameObject item = (GameObject)data["item"];
            var slot = Instantiate(slotObject);
            slots.Add(slot.transform);
             EventManager.TriggerEvent("FollowMe", slot, new EventDict() { { "receiver", item } });
        }
    }
    private void removeSlot(EventDict data)
    {
        if (((GameObject)data["receiver"]) == gameObject)
        {
            GameObject item = (GameObject)data["item"];
            var slot = Instantiate(slotObject);
            slots.RemoveAt(0);
            EventManager.TriggerEvent("UnfollowMe", slot, new EventDict() { { "receiver", item }, { "newTarget", data["newTarget"] } });
            
        }
    }

    private void LateUpdate()
    {
        var offset = slots.Count != 0 ? 2 * Mathf.PI / slots.Count : 0;
        // Debug.Log(offset);
        for (int i = 0; i < slots.Count; i++)
        {
            positionOffset.Set(
                Mathf.Cos(angle + offset * i) * CircleRadius,
                ElevationOffset,
                Mathf.Sin(angle + offset * i) * CircleRadius
            );
            slots[i].position = transform.position + positionOffset;
        }

        angle += Time.deltaTime * RotationSpeed;
    }

    //     private Vector3 ComputePositionOffset(float a)
    //     {
    //         Target = this.transform;
    //         a *= Mathf.Deg2Rad;

    //         // Compute the position of the object
    //         Vector3 positionOffset = new Vector3(
    //             Mathf.Cos(a) * CircleRadius,
    //             ElevationOffset,
    //             Mathf.Sin(a) * CircleRadius
    //         );

    //         // Change position if the object must rotate in the coordinate system of the target
    //         // (i.e in the local space of the target)
    //         if (tranform != null && UseTargetCoordinateSystem)
    //             positionOffset = transform.TransformVector(positionOffset);

    //         return positionOffset;
}

// #if UNITY_EDITOR

//     [SerializeField]
//     private bool drawGizmos = true;

//     private void OnDrawGizmosSelected()
//     {
//         Target = this.transform;
//         if (!drawGizmos)
//             return;

//         // Draw an arc around the target
//         Vector3 position = Target != null ? Target.position : Vector3.zero;
//         Vector3 normal = Vector3.up;
//         Vector3 forward = Vector3.forward;
//         Vector3 labelPosition;

//         Vector3 positionOffset = ComputePositionOffset(StartAngle);
//         Vector3 verticalOffset;


//         if (Target != null && UseTargetCoordinateSystem)
//         {
//             normal = Target.up;
//             forward = Target.forward;
//         }
//         verticalOffset = positionOffset.y * normal;

//         // Draw label to indicate elevation
//         if (Mathf.Abs(positionOffset.y) > 0.1)
//         {
//             UnityEditor.Handles.DrawDottedLine(position, position + verticalOffset, 5);
//             labelPosition = position + verticalOffset * 0.5f;
//             labelPosition += Vector3.Cross(verticalOffset.normalized, Target != null && UseTargetCoordinateSystem ? Target.forward : Vector3.forward) * 0.25f;
//             UnityEditor.Handles.Label(labelPosition, ElevationOffset.ToString("0.00"));
//         }

//         position += verticalOffset;
//         positionOffset -= verticalOffset;

//         UnityEditor.Handles.DrawWireArc(position, normal, forward, 360, CircleRadius);

//         // Draw label to indicate radius
//         UnityEditor.Handles.DrawLine(position, position + positionOffset);
//         labelPosition = position + positionOffset * 0.5f;
//         labelPosition += Vector3.Cross(positionOffset.normalized, Target != null && UseTargetCoordinateSystem ? Target.up : Vector3.up) * 0.25f;
//         UnityEditor.Handles.Label(labelPosition, CircleRadius.ToString("0.00"));
//     }

// #endif
// }
