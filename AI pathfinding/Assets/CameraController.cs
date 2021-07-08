using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject Skeleton;

    //keeps the camera in line with the skeleton
    void Update()
    {
        transform.position = new Vector3(Skeleton.transform.position.x, 0, 0);
    }
}
