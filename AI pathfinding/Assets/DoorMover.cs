using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DoorMover : MonoBehaviour
{

    [SerializeField] private GameObject[] DoorPoint;
    [SerializeField] private GameObject[] StartPoint;
    [SerializeField] private GameObject[] Door;
    [SerializeField] bool open;
    [SerializeField] float setTime;

    void Start()
    {
        OpenClose();
        open = false;

        
    }

    public void Update()
    {
        OpenClose();
        if (Time.time > setTime + 10)
        {
            SetTime();
        }
    }

    private void OpenClose()
    {
        float targetTime = (Time.time - setTime) / 1;
        if (open)
        {
            for (int i = 0; i < Door.Length; i++)
            {
                Door[i].transform.position = Vector3.Lerp(DoorPoint[i].transform.position, StartPoint[i].transform.position, targetTime);
            }
        }
        else
        {
            for (int i = 0; i < Door.Length; i++)
            {
                Door[i].transform.position = Vector3.Lerp(StartPoint[i].transform.position, DoorPoint[i].transform.position, targetTime);
            }
        }
    }

    void SetTime()
    {
        setTime = Time.time;
        if (open)
        {
            open = false;
        }
        else
        {
            open = true;
        }
    }
}
