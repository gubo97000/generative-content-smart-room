using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour
{
    public IEnumerator Shake()
    {
        float counter = 0f;
        Vector3 pos = transform.position;
        float maxX = 0.5f;
        float maxY = 0f;
        float ShakeTime = 0.8f;
        float speed = 20f;

        while (true)
        {
            counter += Time.deltaTime;
            if (counter >= ShakeTime)
            {
                yield break;
            }
            else
            {
                transform.position = pos + new Vector3((ShakeTime - counter) / 10 * Mathf.Sin(Time.time * speed), 0);
            }
            yield return null;
        }
    }
}
