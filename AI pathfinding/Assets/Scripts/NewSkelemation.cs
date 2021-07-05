using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NewSkelemation : MonoBehaviour
{
    [SerializeField] public float speed;
    [SerializeField] private Animator animator;
    [SerializeField] private NavMeshAgent skeleagent;



    void Update()
    {
        //changes the animation based on the navmeshagent speed
        speed = skeleagent.velocity.magnitude;
        Debug.Log(speed);
        if (speed <= 2)
        {
            animator.SetFloat("speed", 0);
        }
        if (speed > 2 && speed < 4)
        {
            animator.SetFloat("speed", 1);
        }
        if (speed >= 4)
        {
            animator.SetFloat("speed", 2);
        }

    }
}
