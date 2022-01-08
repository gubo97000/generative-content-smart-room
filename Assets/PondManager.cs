using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class PondManager : ObjectStateHandler
{
    private int _index = 0;
    public float secondsToMoveWater = 2f;
    public float offset = 1.5f;
    public GameObject pond;
    private Vector3 startPosition;

    //Set the states here, with the scripts attached for each state.
    private void Reset()
    {
        states = new State[]
        {
    new State("Full"),
    new State("Empty")
        };
    }

    //Remember to call the Start function of ObjectStateHandler
    protected override void Start()
    {
        base.Start();
        EventManager.StartListening("SwitchPondState", OnSwitchPondState);

        startPosition = pond.transform.position;
    }

    void OnDestroy()
    {
        EventManager.StopListening("SwitchPondState", OnSwitchPondState);

        // Restores pond level
        if (CurrentState == "Empty")
        {
            MoveWaterLevel(0);
        }
    }

    void OnSwitchPondState(EventDict dict)
    {
        MoveWaterLevel(secondsToMoveWater);

        _index++;
        _index %= states.Length;
        CurrentState = states[_index].name;
    }

    void MoveWaterLevel(float seconds)
    {
        Vector3 finalPosition = startPosition;
        if (CurrentState == "Full")
        {
            finalPosition.y -= offset;
        }
        else if(CurrentState == "Empty")
        {
            finalPosition.y += offset;
        }
        StartCoroutine(MoveOverSeconds(pond, startPosition, finalPosition, seconds));
    }

    public IEnumerator MoveOverSeconds(GameObject objectToMove, Vector3 start, Vector3 end, float seconds)
    {
        float elapsedTime = 0;
        while (elapsedTime < seconds)
        {
            objectToMove.transform.position = Vector3.Lerp(start, end, (elapsedTime / seconds));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        objectToMove.transform.position = end;
    }
}
