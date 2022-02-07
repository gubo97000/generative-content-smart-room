using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

[ExecuteAlways]
public class PondManager : ObjectStateHandler
{
    public float secondsToMoveWater = 2f;
    public float offset = 1.5f;
    public GameObject pond;
    public GameObject honeyPond;

    //Set the states here, with the scripts attached for each state.
    private void Reset()
    {
        states = new State[]
        {
    new State("Full"),
    new State("Empty"),
    new State("Honey")
        };
    }

    //Remember to call the Start function of ObjectStateHandler
    protected override void Start()
    {
        base.Start();
        EventManager.StartListening("EmptyPond", EmptyPond);
        EventManager.StartListening("WaterPond", WaterPond);
        EventManager.StartListening("HoneyPond", HoneyPond);
    }

    void OnDestroy()
    {
        EventManager.StopListening("EmptyPond", EmptyPond);
        EventManager.StopListening("WaterPond", WaterPond);
        EventManager.StopListening("HoneyPond", HoneyPond);
        
        //Destroy(pond.GetComponent<AudioSource>());

        // Restores pond level
        if (CurrentState != "Full")
        {
            MoveWaterLevel(0);
        }

        if (CurrentState == "Honey")
        {
            MoveHoneyLevel(0);
        }
    }

    void EmptyPond(EventDict dict)
    {
        MoveWaterLevel(secondsToMoveWater);

        CurrentState = "Empty";
        
        //FindObjectOfType<AudioManager>().Remove(pond);
    }
    
    void WaterPond(EventDict dict)
    {
        if (CurrentState == "Honey")
        {
            MoveHoneyLevel(secondsToMoveWater);
        }

        MoveWaterLevel(secondsToMoveWater);

        CurrentState = "Full";
        
        //FindObjectOfType<AudioManager>().Add(pond,"Water");

    }

    void HoneyPond(EventDict dict)
    {
        if (CurrentState == "Empty")
        {
            MoveHoneyLevel(secondsToMoveWater);

            CurrentState = "Honey";
        }

        //FindObjectOfType<AudioManager>().Add(pond, "Water");
    }

    void MoveWaterLevel(float seconds)
    {
        Vector3 startPosition = pond.transform.position;
        Vector3 finalPosition = startPosition;
        if (CurrentState == "Full")
        {
            finalPosition.y -= offset;
        }
        else
        {
            finalPosition.y += offset;
        }
        if(gameObject.activeInHierarchy) StartCoroutine(MoveOverSeconds(pond, startPosition, finalPosition, seconds));
    }

    void MoveHoneyLevel(float seconds)
    {
        Vector3 startPosition = honeyPond.transform.position;
        Vector3 finalPosition = startPosition;
        if (CurrentState == "Honey")
        {
            finalPosition.y -= offset / 2;
        }
        else
        {
            finalPosition.y += offset / 2;
        }
        if (gameObject.activeInHierarchy) StartCoroutine(MoveOverSeconds(honeyPond, startPosition, finalPosition, seconds));
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
