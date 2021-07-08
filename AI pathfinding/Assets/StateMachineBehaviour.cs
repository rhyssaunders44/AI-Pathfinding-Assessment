using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StateMachineBehaviour : MonoBehaviour
{
    [SerializeField] public NavMeshAgent agent;
    [SerializeField] private GameObject[] key;
    [SerializeField] private GameObject[] door;
    [SerializeField] private GameObject[] exit;
    [SerializeField] private bool switchStates;
    [SerializeField] private States skeleState;
    [SerializeField] private float[] distancetoobjects;
    [SerializeField] private bool[] visited;
    [SerializeField] private int lastDoor;
    [SerializeField] private int lastKey;
    [SerializeField] private Vector3 lastPosition;
    [SerializeField] private float minMoveDistance = 0.3f;
    [SerializeField] private int keysFound;
    [SerializeField] private GameObject bully;
    [SerializeField] public States lastState;
    int x;


    private void Start()
    {
        visited = new bool[] {false, false, false, false, false };
        lastDoor = 1000;
        lastKey = 1000;
        distancetoobjects = new float[5];

        StartCoroutine(routine: PositionCheck());
        skeleState = States.Escaping;
        switchStates = true;

    }


    public void Update()
    {
        if (switchStates)
        {
            SetState(skeleState);
        }

        if(skeleState == States.Fleeing)
        {
            if(Vector3.Distance(agent.transform.position, agent.destination) < minMoveDistance)
            {
                skeleState = lastState;
                switchStates = true;
            }
        }
    }


    public void SetState(States currentState)
    {
            switchStates = false;   
        switch (currentState)
        {
            
            case States.Searching:
                if(keysFound >= 3)
                {
                    skeleState = States.Escaping;
                    switchStates = true;
                }
                Debug.Log("searchmode");
                AquirePoint(key, lastKey, 1);
                break;

            case States.Switching:

                AquirePoint(door, lastDoor, 0);
                lastDoor = x;
                Debug.Log("switchmode");
                break;

            case States.Fleeing:
                
                StartCoroutine(routine: Flee());
                Debug.Log("fleemode");
                break;

            case States.Escaping:
                if(keysFound < 3)
                {
                    skeleState = States.Searching;
                    switchStates = true;
                }
                AquirePoint(exit, 1, 2);
                Debug.Log("escapemode");
                break;

            case States.Idle:
                Debug.Log("idleMode");
                break;
        }
    }

    public void AquirePoint(GameObject[] target, int lastobject, int searchtype)
    {
        Debug.Log("LastObject" + lastobject);
        float smallest = 100;

        for (int i = 0; i < target.Length; i++)
        {

            if (target[i].activeSelf)
            {
                distancetoobjects[i] = Vector3.Distance(agent.transform.position, target[i].transform.position);
            }
            else
            {
                distancetoobjects[i] = 10000;
            }

            if(searchtype == 0)
            {
                if (visited[i])
                    distancetoobjects[i] += 1000;
            }

            if(i == lastobject)
            {
                distancetoobjects[i] += 1000;
            }

            if ((distancetoobjects[i] < smallest) && distancetoobjects[i] != 0)
            {
                smallest = distancetoobjects[i];
                x = i;
            }

            Debug.Log("Distance: " + Vector3.Distance(agent.transform.position, target[i].transform.position));
        }

        Debug.Log("Choosing: " + x);

        agent.SetDestination(target[x].transform.position);
        if (searchtype == 0)
        {
            lastDoor = x;
        }
        if(searchtype == 1)
        {
            lastKey = x;
        }
        else 
        return;
    }

    public IEnumerator PositionCheck()
    {
        lastPosition = agent.transform.position;

        yield return new WaitForSeconds(1f);

        if (Vector3.Distance(agent.transform.position, lastPosition) < minMoveDistance )
        {
            if(lastState == States.Fleeing)
            {
                skeleState = States.Searching;
                switchStates = true;
                
            }
            if(lastState == States.Escaping)
            {
                skeleState = States.Escaping;
                switchStates = true;
                
            }
            else
            {
                skeleState = States.Switching;
                lastKey = 30;
                switchStates = true;
                
            }

        }
        StartCoroutine(routine: PositionCheck());
        yield return null;
    }


    public enum States
    {
        Searching, Switching, Escaping, Fleeing, Idle
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "Key":
                keysFound += 1;
                if (keysFound >= 3)
                {
                    skeleState = States.Escaping;
                    switchStates = true;
                }
                other.gameObject.SetActive(false);

                AquirePoint(key, lastKey, 1);
                break;
            case "Switch":
                if (keysFound >= 3)
                {
                    skeleState = States.Escaping;
                }
                else
                {
                    skeleState = States.Searching;
                }

                visited[x] = true;
                switchStates = true;
                break;
            case "Exit":
                StopAllCoroutines();
                skeleState = States.Idle;
                switchStates = true;
                break;
            case "Enter":
                for (int i = 0; i < visited.Length; i++)
                {
                    visited[i] = false;
                }
                keysFound = 0;
                skeleState = States.Idle;
                switchStates = true;
                break;
            case "bully":
                lastState = skeleState;
                skeleState = States.Fleeing;
                switchStates = true;
                break;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "bully")
        {
            skeleState = States.Fleeing;
            switchStates = true;
        }
    }


    public IEnumerator Return()
    {
        agent.SetDestination(exit[1].transform.position);
        yield return new WaitForSeconds(10);

        for (int i = 0; i < key.Length; i++)
        {
            key[i].SetActive(true);
            distancetoobjects[i] = 0;
            visited[i] = false;
        }
        x = 0;
        yield break;
    }

    public void ReturnButton()
    {
        StartCoroutine(routine: Return());
        StartCoroutine(routine: PositionCheck());
    }

    public IEnumerator Flee()
    {
        if(skeleState != States.Fleeing)
        {
            lastState = skeleState;
        }

        Transform startTransform = agent.transform;
        transform.rotation = Quaternion.LookRotation(transform.position - bully.transform.position);

        Vector3 runTo = transform.position + transform.forward * 2;

        NavMeshHit hit;  
        NavMesh.SamplePosition(runTo, out hit, 3, 1 << NavMesh.GetAreaFromName("Walkable"));


        // reset the transform back to our start transform
        transform.position = startTransform.position;
        transform.rotation = startTransform.rotation;

        agent.SetDestination(hit.position);

        yield break;
    }

}
