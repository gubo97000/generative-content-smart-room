using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Campfire : MonoBehaviour
{
    enum State
    {
        COLLECTING,
        TRIGGERABLE

    }
    State state = State.COLLECTING;
    int collected = 0;
    public int toCollect = 3;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (state != State.COLLECTING) return;
        if (other.gameObject.tag == "Branch")
        {
            Destroy(other.gameObject);
            collected++;
            if (collected == toCollect)
            {
                state = State.TRIGGERABLE;
            }

        }
    }
}
