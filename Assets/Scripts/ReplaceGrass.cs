using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplaceGrass : MonoBehaviour
{
    public GameObject flowers;

    void GrowFlowers()
    {
        flowers.SetActive(true);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Seed")
        {
            Debug.Log("Seed has touched the grass");
            GrowFlowers();
            Destroy(other.gameObject);
        }
    }
}