using UnityEngine;
using System.Collections;

public class Dude : MonoBehaviour
{
    Animator animatorThing;
    public Camera ChaseCamera;

    // Use this for initialization
    void Start()
    {
        animatorThing = GetComponent<Animator>();


    }

    // Update is called once per frame
    void Update()
    {
        animatorThing.SetFloat("Speed", Input.GetAxis("Vertical"));

        //if (Input.GetAxis("Vertical") > 0)
        //{ 
        //}
        //if (Input.GetAxis("Vertical") < 0)
        //{
        //}

        ChaseCamera.transform.position = transform.position + new Vector3(0, 7.5f, -7.5f);
        ChaseCamera.transform.LookAt(transform.position);
    }
}
