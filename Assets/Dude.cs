using System;
using UnityEngine;
using System.Collections;

public class Dude : MonoBehaviour
{
    Animator animatorThing;
    public Camera ChaseCamera;
    public float WalkSpeed;

    private Vector2 moving;
    private Vector2 facing;
    private float turnSpeed;

    private CharacterController playerController;

    // Use this for initialization
    void Start()
    {
        playerController = GetComponent<CharacterController>();
        animatorThing = GetComponent<Animator>();
        moving = new Vector2(0, 1f);
        turnSpeed = 5f;
    }

    // Update is called once per frame
    private void Update()
    {
        moving = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        if (Math.Abs(Input.GetAxis("Horizontal")) > 0.1f || Math.Abs(Input.GetAxis("Vertical")) > 0.1f)
            facing = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        animatorThing.SetBool("Firing", Input.GetButton("Fire1"));

        animatorThing.SetFloat("Speed", Vector3.ClampMagnitude(moving, 1f).magnitude);

        var targetYaw = Mathf.Atan2(facing.x, facing.y)*Mathf.Rad2Deg;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(transform.eulerAngles.x, targetYaw, transform.eulerAngles.z), turnSpeed*Time.deltaTime);

        var aimAtPosition = GetScreenPointInWorldPlane(Input.mousePosition, 0f);
        var toAimPosition = aimAtPosition - transform.position;
        var aimYaw = Quaternion.LookRotation(toAimPosition).eulerAngles.y;
        var relativeAimYaw = aimYaw - transform.eulerAngles.y;

        var velocity = Vector3.ClampMagnitude(new Vector3(moving.x, 0f, moving.y), 1f)*WalkSpeed;

        if (moving.magnitude > 0.1f)
            playerController.Move(velocity*Time.deltaTime);

        ChaseCamera.transform.position = transform.position + new Vector3(0, 7.5f, -7.5f);
        ChaseCamera.transform.LookAt(transform.position);
    }

    private Vector3 GetScreenPointInWorldPlane(Vector3 screenPoint, float height)
    {
        var ray = Camera.main.ScreenPointToRay(screenPoint);
        var worldPlane = new Plane();
        var dist = 0f;
        worldPlane.SetNormalAndPosition(Vector3.up, new Vector3(0, height, 0));
        worldPlane.Raycast(ray, out dist);
        return ray.GetPoint(dist);
    }
}
