using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;



public class KinectPlayerBinderManager : MonoBehaviour
{
    [System.Serializable] public struct ListOfGameObjects { public List<GameObject> list; }
    public List<ListOfGameObjects> toBind;
    private ulong[] pastIDs = new ulong[6] { 1, 2, 3, 4, 5, 6 }; //This could cause bugs if toBind is grows during runtime
    private Vector3 standardizedFloorSize;
    private Vector3 sensorDisallinment;

    // Start is called before the first frame update
    void Start()
    {
        if (MagicRoomManager.instance != null)
        {
            MagicRoomManager.instance.MagicRoomKinectV2Manager.Skeletons += ManageSkeleton;

            MagicRoomManager.instance.MagicRoomKinectV2Manager.StartStreamingSkeletons(250);

            if (MagicRoomManager.instance.systemConfiguration != null)
            {
                sensorDisallinment = new Vector3(MagicRoomManager.instance.systemConfiguration.floorOffsetX, MagicRoomManager.instance.systemConfiguration.floorOffsetY, 0);
                standardizedFloorSize = new Vector3(MagicRoomManager.instance.systemConfiguration.floorSizeX / Mathf.Pow(2.74f, 2), 1, MagicRoomManager.instance.systemConfiguration.floorSizeY / Mathf.Pow(2.88f, 2));
            }
        }
        // pastDistances = new ulong[toBind.Count]{};
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void ManageSkeleton(Dictionary<ulong, Skeleton> skel)
    {

        // ulong closestId = skel.Keys.ToArray()[0];
        // float distance = Mathf.Infinity;
        Dictionary<ulong, float> playersDistances = new Dictionary<ulong, float>();
        foreach (ulong id in skel.Keys)
        {
            playersDistances.Add(id, skel[id].SpineBase.z);
        }
        // var sorted = (from pair in playersDistances orderby pair.Value ascending select pair);
        //Getting first N positions
        var positionsConsidered = (from pair in playersDistances orderby pair.Value ascending select pair.Key).Take(toBind.Count);
        var pastIDsConsidered = pastIDs.Take(toBind.Count).ToList<ulong>();
        //Find new IDs
        foreach (var playerId in positionsConsidered)
        {
            if (!pastIDsConsidered.Contains(playerId))
            {
                //Find first available spot
                for (int i = 0; i < toBind.Count; i++)
                {
                    if (!positionsConsidered.Contains(pastIDsConsidered[i]))
                    {
                        BindId(playerId, i);
                    }
                }
            }
        }
    }
    private void BindId(ulong id, int indexOfToBind)
    {
        pastIDs[indexOfToBind] = id;
        foreach (var item in toBind[indexOfToBind].list)
        {
            item.BroadcastMessage("OnChangeKinectPlayerId", id);
        }
    }
}
