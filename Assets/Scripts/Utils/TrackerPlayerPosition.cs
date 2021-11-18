using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[Serializable] public class PlayerPositiondDict : SerializableDictionary<PartToTrack, Transform> { }

public class TrackerPlayerPosition : MonoBehaviour
{
    public StringEvent HandStateRight, HandStateLeft;

    public StringEvent GestureState;

    public bool activateHandCloseEvents = false;
    public bool activateGestureEvents = false;
    public Vector3Bool activeTrackAxis;
    public PlayerToFollow playerIdentifier = PlayerToFollow.Closest_player;

    public PlayerPositiondDict trackingSchema = new PlayerPositiondDict();

    private static int numActiveTracker = 0;
    private Skeleton skelPosition;

    private bool leftHandState;
    private bool rightHandState;

    private Vector3 standardizedFloorSize;
    private Vector3 sensorDisallinment;
    public BoxArea interactiveArea;

    private void Start()
    {
        if (MagicRoomManager.instance != null)
        {
            MagicRoomManager.instance.MagicRoomKinectV2Manager.Skeletons += ManageSkeleton;
            numActiveTracker++;
            if (numActiveTracker == 1)
            {
                MagicRoomManager.instance.MagicRoomKinectV2Manager.StartStreamingSkeletons(250);
            }
            if (MagicRoomManager.instance.systemConfiguration != null)
            {
                sensorDisallinment = new Vector3(MagicRoomManager.instance.systemConfiguration.floorOffsetX, MagicRoomManager.instance.systemConfiguration.floorOffsetY, 0);
                standardizedFloorSize = new Vector3(MagicRoomManager.instance.systemConfiguration.floorSizeX / Mathf.Pow(2.74f, 2), 1, MagicRoomManager.instance.systemConfiguration.floorSizeY / Mathf.Pow(2.88f, 2));
            }
        }
        HandStateRight = new StringEvent();
        HandStateLeft = new StringEvent();
    }

    private void Update()
    {
        if (skelPosition != null)
        {
            foreach (var p in trackingSchema)
            {
                Vector3 newposition;
                switch (p.Key)
                {
                    case PartToTrack.BodyOverallPosition: newposition = skelPosition.SpineBase; break;
                    case PartToTrack.Head: newposition = skelPosition.Head; break;
                    case PartToTrack.LeftAnkle: newposition = skelPosition.AnkleLeft; break;
                    case PartToTrack.RightAnkle: newposition = skelPosition.AnkleRight; break;
                    case PartToTrack.LeftElbow: newposition = skelPosition.ElbowLeft; break;
                    case PartToTrack.RightElbow: newposition = skelPosition.ElbowRight; break;
                    case PartToTrack.LeftFoot: newposition = skelPosition.FootLeft; break;
                    case PartToTrack.RightFoot: newposition = skelPosition.FootRight; break;
                    case PartToTrack.LeftHand: newposition = skelPosition.HandLeft; break;
                    case PartToTrack.RightHand: newposition = skelPosition.HandRight; break;
                    case PartToTrack.LeftHip: newposition = skelPosition.HipLeft; break;
                    case PartToTrack.RightHip: newposition = skelPosition.HipRight; break;
                    case PartToTrack.RightShoulder: newposition = skelPosition.ShoulderRight; break;
                    case PartToTrack.LeftShoulder: newposition = skelPosition.ShoulderRight; break;
                    case PartToTrack.LeftWrist: newposition = skelPosition.WristLeft; break;
                    case PartToTrack.RightWrist: newposition = skelPosition.WristRight; break;
                    case PartToTrack.SpineBase: newposition = skelPosition.SpineBase; break;
                    case PartToTrack.SpineMid: newposition = skelPosition.SpineMid; break;
                    case PartToTrack.Neck: newposition = skelPosition.Neck; break;
                    case PartToTrack.KneeLeft: newposition = skelPosition.KneeLeft; break;
                    case PartToTrack.KneeRight: newposition = skelPosition.KneeRight; break;
                    case PartToTrack.SpineShoulder: newposition = skelPosition.SpineShoulder; break;
                    case PartToTrack.HandTipLeft: newposition = skelPosition.HandTipLeft; break;
                    case PartToTrack.ThumbLeft: newposition = skelPosition.ThumbLeft; break;
                    case PartToTrack.HandTipRight: newposition = skelPosition.HandTipRight; break;
                    case PartToTrack.ThumbRight: newposition = skelPosition.ThumbRight; break;
                    default: newposition = Vector3.zero; break;
                }

                Vector3 axis = new Vector3(Convert.ToSingle(activeTrackAxis.X), Convert.ToSingle(activeTrackAxis.Y), Convert.ToSingle(activeTrackAxis.Z));

                //rescale according to the standard room size
                //1) Remove the misallignemnt due to the misplacement of the sensor in the physical room
                newposition = newposition - sensorDisallinment;
                //2) rescale the position in te standard measurement
                newposition = Vector3.Scale(newposition, standardizedFloorSize);
                //3) Transform into the coordiantes in the game
                newposition = Vector3.Scale(newposition, interactiveArea.getScaleVector()) + interactiveArea.getOrinig();
                //4)apply filters for the axis
                newposition = Vector3.Scale(newposition, axis);
                //5) assign the computed position
                trackingSchema[p.Key].position = newposition;

                if (activateHandCloseEvents)
                {
                    //Dictionary<PartToTrack, bool> hand = new Dictionary<PartToTrack, bool>();

                    //bool haschangehappened = false;
                    if (skelPosition.IsLeftHandClosed() && !leftHandState)
                    {
                        //hand.Add(PartToTrack.LeftHand, true);
                        leftHandState = true;
                        //haschangehappened = true;
                        HandStateLeft?.Invoke("LEFTHAND_OPEN");
                    }
                    if (!skelPosition.IsLeftHandClosed() && leftHandState)
                    {
                        //hand.Add(PartToTrack.LeftHand, false);
                        leftHandState = true;
                        //haschangehappened = true;

                        HandStateLeft?.Invoke("LEFTHAND_CLOSE");
                    }
                    if (skelPosition.IsRightHandClosed() && !rightHandState)
                    {
                        //hand.Add(PartToTrack.RightHand, true);
                        rightHandState = true;
                        HandStateRight?.Invoke("RIGHTHAND_OPEN");
                        //haschangehappened = true;
                    }
                    if (!skelPosition.IsRightHandClosed() && rightHandState)
                    {
                        //hand.Add(PartToTrack.RightHand, false);
                        HandStateRight?.Invoke("RIGHTHAND_CLOSE");
                        rightHandState = true;
                        //haschangehappened = true;
                    }
                    /*if (haschangehappened)
                    {
                        HandState?.Invoke(hand);
                    }*/
                }
            }
            if (activateGestureEvents)
            {
                if (skelPosition.Gestures.Length > 0)
                {
                    foreach (string s in skelPosition.Gestures)
                    {
                        GestureState?.Invoke(s);
                    }
                }
            }
        }
    }

    private void ManageSkeleton(Dictionary<ulong, Skeleton> skel)
    {
        if (playerIdentifier != PlayerToFollow.Closest_player)
        {
            Debug.Log(System.Convert.ToUInt64(playerIdentifier));
            foreach (ulong id in skel.Keys)
            {
                if (id == System.Convert.ToUInt64(playerIdentifier))
                {
                    skelPosition = skel[id];
                }
            }
        }
        else
        {
            ulong closestId = skel.Keys.ToArray()[0];
            float distance = Mathf.Infinity;
            foreach (ulong id in skel.Keys)
            {
                if (skel[id].SpineBase.z < distance && skel[id].SpineBase.z > 0)
                {
                    closestId = id;
                    distance = skel[id].SpineBase.z;
                }
            }
            skelPosition = skel[closestId];
        }
    }

    private void OnDestroy()
    {
        if (MagicRoomManager.instance != null)
        {
            if (numActiveTracker > 0)
            {
                MagicRoomManager.instance.MagicRoomKinectV2Manager.Skeletons -= ManageSkeleton;
                numActiveTracker--;
                if (numActiveTracker == 0)
                {
                    MagicRoomManager.instance.MagicRoomKinectV2Manager.StopStreamingSkeletons();
                }
            }
        }
    }
}

public enum PlayerToFollow
{
    Closest_player, Player1, Player2, Player3, Player4, Player5, Player6
}

public enum PartToTrack
{
    None, BodyOverallPosition, Head, LeftHand, RightHand, LeftFoot, RightFoot, RightAnkle, LeftAnkle, LeftHip, RightHip, RightShoulder, LeftShoulder, LeftElbow, RightElbow, LeftWrist, RightWrist,
    SpineBase, SpineMid, Neck, KneeLeft, KneeRight, SpineShoulder, HandTipLeft, ThumbLeft, HandTipRight, ThumbRight
}

[Serializable]
public class Vector3Bool
{
    public bool X;
    public bool Y;
    public bool Z;
}

[Serializable]
public class BoxArea
{
    public Vector3 center = Vector3.zero;
    public float width = 1;
    public float depth = 1;
    public float height = 1;

    internal Vector3 getScaleVector()
    {
        return new Vector3(width, height, -depth);
    }

    internal Vector3 getOrinig()
    {
        return center + Vector3.forward * depth / 2;
    }
}

[System.Serializable]
public class HandgestureEvent : UnityEvent<handstate>
{
}

public enum handstate
{
    RIGHTHAND_OPEN, RIGHTHAND_CLOSE, LEFTHAND_OPEN, LEFTHAND_CLOSE
}