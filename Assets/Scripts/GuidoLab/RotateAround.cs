// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class RotateAround : MonoBehaviour
// {

//     public float RotationSpeed = 1;

//     public float CircleRadius = 3;

//     public float ElevationOffset = 0;

//     private Vector3 positionOffset;
//     private float angle;

//     public List<Transform> slots;

//     private void LateUpdate()
//     {
//         var offset = 360 / slots.Count;
//         foreach (var slot in slots)
//         {
//             positionOffset.Set(
//                 Mathf.Cos(angle + offset) * CircleRadius,
//                 ElevationOffset,
//                 Mathf.Sin(angle + offset) * CircleRadius

//             );
//             slot.position = transform.position + positionOffset;
//         }

//         angle += Time.deltaTime * RotationSpeed;
//     }
// }
