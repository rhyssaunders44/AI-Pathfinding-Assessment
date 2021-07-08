using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spiiin : MonoBehaviour
{
    public Animator animator;
    public GameObject parent;
    float speed = 0.5f;
    [SerializeField] public int keyID;

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3( transform.position.x , 0.3f + Mathf.PingPong(Time.time, 0.7f) , transform.position.z );
        transform.Rotate(Vector3.up * speed);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "BonyBoy")
        {

        }
    }
}
