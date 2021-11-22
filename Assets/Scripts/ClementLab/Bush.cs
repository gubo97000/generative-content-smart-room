using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bush : MonoBehaviour
{
    public bool Awake;
    // Start is called before the first frame update
    void Start()
    {
        this.Awake = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void OnMouseDown(){
        if (this.Awake) {
            this.CollectSeed();
            
            StartCoroutine(waiter());
            
        }
    }
    
    IEnumerator waiter()
    {
        this.Awake = false;
        yield return new WaitForSeconds(30);
        this.Awake = true;
    }
    
    public void CollectSeed()
    {
        Debug.Log("You collected 2-3 seeds");
    }
}
