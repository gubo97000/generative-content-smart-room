using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EatFish : MonoBehaviour
{
    public float speed = 1.0f;

    private GameObject prey;
    private bool hasEatenFish = false;

    public void setPrey(GameObject p)
    {
        prey = p;
    }

    void Update()
    {
        if (prey != null)
        {

            transform.position = Vector3.MoveTowards(transform.position, prey.transform.position, speed * Time.deltaTime);
            transform.rotation = Quaternion.LookRotation(prey.transform.position - transform.position);

            if (Vector3.Distance(transform.position, prey.transform.position) < 0.5 && !hasEatenFish)
            {
                Destroy(prey);
                EventManager.TriggerEvent("EatFish", gameObject);
                hasEatenFish = true;

                // fly away
                var go = new GameObject();
                go.transform.position = new Vector3(-1000, 100, 0);
                prey = go;
            }
        }
    }
}
