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


    //all of the available states the state machine has
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


    //locates the next point.
    public void AquirePoint(GameObject[] target, int lastobject, int searchtype)
    {
        Debug.Log("LastObject" + lastobject);
        float smallest = 100;

        for (int i = 0; i < target.Length; i++)
        {

            // if the target is active in the scene
            if (target[i].activeSelf)
            {
                distancetoobjects[i] = Vector3.Distance(agent.transform.position, target[i].transform.position);
            }
            else
            {
                //if not shove it down the priority list
                distancetoobjects[i] = 10000;
            }

            if(searchtype == 0)
            {
                //if its a visited door shove it down the priority list
                if (visited[i])
                    distancetoobjects[i] += 1000;
            }

            if(i == lastobject)
            {
                //if it was the last object visited shove it down the priority list
                distancetoobjects[i] += 1000;
            }

            // the closest available object is the target
            // i wishi i could have used the calculate path but i couldnt get it to work
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

        //checks to see if the AI is stuck, currently over active as the exit from fleeing doesnt work as intended
        //and it will always go for a switch after but it still works.
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

    // our ai states
    public enum States
    {
        Searching, Switching, Escaping, Fleeing, Idle
    }

    //the various triggers the AI may hit, and what to do if it does hit
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

    //if it hits the bully and cant escape keep trying to set a new point to run to
    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "bully")
        {
            skeleState = States.Fleeing;
            switchStates = true;
        }
    }

    //be free to run hom
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

    //interactable 
    public void ReturnButton()
    {
        StartCoroutine(routine: Return());
        StartCoroutine(routine: PositionCheck());
    }

    //run my boy
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
