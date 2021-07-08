using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpener : MonoBehaviour
{
    [SerializeField] private GameObject[] doorObjects;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "BonyBoy")
        {
            doorObjects[0].transform.position = doorObjects[1].transform.position;
        }
    }
}
