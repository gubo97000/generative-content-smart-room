using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteAlways]
public class SwitchTree : MonoBehaviour
{
    public GameObject spawner;
    public GameObject[] trees;
    private GameObject instance;
    private int index = 0;

    void Start()
    {
        instance = Instantiate(trees[0], spawner.transform.position, Quaternion.identity);
    }

    public void trigger()
    {
        index = (index + 1) % trees.Length;
        Destroy(instance);
        instance = Instantiate(trees[index], spawner.transform.position, Quaternion.identity);

        Debug.Log("Touched the fairy");
    }
}
