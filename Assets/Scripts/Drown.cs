using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script is applied to the carps in the pond that will react to the pond state switch
public class Drown : MonoBehaviour
{
    private bool isFlipped = false;
    public float seconds = 1f;
    public int rotateOffset = 90;
    public float moveOffset = 0.3f;

    // Start is called before the first frame update
    void Start()
    {
        EventManager.StartListening("WaterPond", WaterPond);
        EventManager.StartListening("HoneyPond", HoneyPond);
        EventManager.StartListening("EmptyPond", EmptyPond);
    }

    void OnDestroy()
    {
        EventManager.StopListening("WaterPond", WaterPond);
        EventManager.StopListening("HoneyPond", HoneyPond);
        EventManager.StopListening("EmptyPond", EmptyPond);
    }

    void WaterPond(EventDict dict)
    {
        OnSwitchPondState(false);
    }
    
    void HoneyPond(EventDict dict)
    {
        OnSwitchPondState(false);
    }
    
    void EmptyPond(EventDict dict)
    {
        OnSwitchPondState(true);
    }
    
    void OnSwitchPondState(bool drown)
    {
        if (isFlipped != drown)
        {
            GetComponent<FollowThePath>().triggerEnabled();

            Rotate();
            Sink();

            isFlipped = drown;
        }
    }

    void Sink()
    {
        Vector3 startPosition = gameObject.transform.position;
        Vector3 finalPosition = startPosition;
        if (!isFlipped)
        {
            finalPosition.y -= moveOffset;
        }
        else
        {
            finalPosition.y += moveOffset;
        }
        StartCoroutine(MoveOverSeconds(gameObject, startPosition, finalPosition, seconds));
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

    void Rotate()
    {
        Quaternion startRotation = gameObject.transform.rotation;
        Quaternion finalRotation;
        if (!isFlipped)
        {
            finalRotation = startRotation * Quaternion.Euler(0, 0, rotateOffset);
        }
        else
            finalRotation = startRotation * Quaternion.Euler(0, 0, -rotateOffset);

        StartCoroutine(RotateOverSeconds(gameObject, startRotation, finalRotation, seconds));
    }

    public IEnumerator RotateOverSeconds(GameObject objectToRotate, Quaternion start, Quaternion end, float seconds)
    {
        float elapsedTime = 0;
        while (elapsedTime < seconds)
        {
            objectToRotate.transform.rotation = Quaternion.Lerp(start, end, (elapsedTime / seconds));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        objectToRotate.transform.rotation = end;
    }
}
