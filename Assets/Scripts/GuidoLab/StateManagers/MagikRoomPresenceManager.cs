using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class MagikRoomPresenceManager : ObjectStateHandler
{
    private void Reset()
    {
        states = new State[]
        {
    new State("InMagik"),
    new State("NotInMagik")
        };
    }

    //Remember to call the Start function of ObjectStateHandler
    protected override void Start()
    {
        base.Start();
        if (MagicRoomManager.instance?.MagicRoomKinectV2Manager != null)
        {
            CurrentState = "InMagik";
        }
        else
        {
            CurrentState = "NotInMagik";
        }
    }
}
