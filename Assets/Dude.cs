using UnityEngine;
using System.Collections;

public class Dude : MonoBehaviour
{
    Animator animatorThing;
    public Camera ChaseCamera;

    private Vector2 facing;

    // Use this for initialization
    void Start()
    {
        animatorThing = GetComponent<Animator>();
        facing = new Vector2(0, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        facing = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        animatorThing.SetFloat("Speed", facing.normalized.magnitude);

        var targetYaw = Mathf.Atan2(facing.x, facing.y)*Mathf.Rad2Deg;
        Debug.Log(facing + " - " + targetYaw);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(transform.eulerAngles.x, targetYaw, transform.eulerAngles.z), Time.deltaTime);

        ChaseCamera.transform.position = transform.position + new Vector3(0, 7.5f, -7.5f);
        ChaseCamera.transform.LookAt(transform.position);
    }
}
