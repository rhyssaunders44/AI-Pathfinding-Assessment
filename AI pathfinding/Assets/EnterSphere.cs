using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterSphere : MonoBehaviour
{
    [SerializeField] private GameObject[] Door;
    [SerializeField] private GameObject[] doororigin;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "BonyBoy")
        {
            RaiseDoors();
        }
    }

    public void RaiseDoors()
    {
        for (int i = 0; i < Door.Length; i++)
        {
            Door[i].transform.position = doororigin[i].transform.position;
        }
    }
}
