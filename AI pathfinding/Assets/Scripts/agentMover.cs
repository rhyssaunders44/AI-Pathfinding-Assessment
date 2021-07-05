using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class agentMover : MonoBehaviour
{
    public NavMeshAgent agent;
    public GameObject agentBody;
    public GameObject[] WayPoint;
    public int wayPointIndex;

    public Transform target;
    private NavMeshPath path;

    private void Start()
    {
        NewWayPoint(0);
    }

    private void Update()
    {
        if (Vector3.Distance(agentBody.transform.position, target.position) < 1f)
        {
            NewWayPoint(wayPointIndex);
        }
    }

    //go to the next waypoint in a loop
    public virtual void NewWayPoint(int currentWayPoint)
    {
        wayPointIndex++;
        if (wayPointIndex >= WayPoint.Length)
            wayPointIndex = 0;

        target = WayPoint[wayPointIndex].transform;
        agent.SetDestination(WayPoint[wayPointIndex].transform.position);
    }
}


