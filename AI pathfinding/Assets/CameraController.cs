using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject Skeleton;

    void Update()
    {
        transform.position = new Vector3(Skeleton.transform.position.x, 0, 0);
    }
}
