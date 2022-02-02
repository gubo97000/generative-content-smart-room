using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class TreesSwitcher : ObjectStateHandler
{
    private int _index = 0;
    public GameObject spawnParticles;
    
    //Set the states here, with the scripts attached for each state.
    private void Reset()
    {
        states = new State[]
        {
            new State("Oak"),
            new State("Apple"),
            new State("Honey")
        };
    }

    //Remember to call the Start function of ObjectStateHandler
    protected override void Start()
    {
        base.Start();
        EventManager.StartListening("SwitchTreesForward", OnSwitchTreeForward);
    }
    void OnDestroy()
    {
        EventManager.StopListening("SwitchTreesForward", OnSwitchTreeForward);
    }

    //Messages from other scripts activate functions, 
    //to change the state change the value of CurrentState
    void OnSwitchTreeForward(EventDict dict)
    {
        StartCoroutine(Switch());
    }

    IEnumerator Switch()
    {
        Vector3 position = transform.position;
        //position.y = 1;

        GameObject sp = Instantiate(spawnParticles, /*position +*/ new Vector3(1.71f, 7.07f, -2.27f), Quaternion.identity);
        sp.GetComponent<ParticleSystem>().Play();
        yield return new WaitForSeconds(2.5f);

        _index++;
        _index %= states.Length;
        CurrentState = states[_index].name;

        sp.GetComponent<ParticleSystem>().Stop();
        yield return new WaitForSeconds(5f);
        Destroy(sp);
    }
}
