using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveStyle : agentMover
{
    //picks a random waypoint to move to
    public override void NewWayPoint(int currentWayPoint)
    {
        base.NewWayPoint(currentWayPoint);

        wayPointIndex = Random.Range(0, WayPoint.Length);

        target = WayPoint[wayPointIndex].transform;
        agent.SetDestination(WayPoint[wayPointIndex].transform.position);
    }


}
