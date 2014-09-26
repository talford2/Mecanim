using System;
using UnityEngine;

public class Dude : MonoBehaviour
{
    private Animator animatorThing;
    public Camera ChaseCamera;
    public float WalkSpeed;
    public float ReloadTime;

    // Movement
    private Vector2 moving;
    private Vector2 facing;
    private float acceleration;
    private float turnSpeed;
    private float aimYaw;
    private float relativeAimYaw;

    // Reloading
    private bool isReloading;
    private float reloadCooldown;

    // Measure Speed
    private Vector3 lastUpdatePosition;
    private float lastUpdateTime;

    private CharacterController playerController;

    private void Awake()
    {
        playerController = GetComponent<CharacterController>();
        animatorThing = GetComponent<Animator>();
        moving = new Vector2(0, 1f);
        acceleration = 10f;
        turnSpeed = 5f;
    }

    private void Update()
    {
        if (Input.GetButton("Reload"))
        {
            isReloading = true;
            reloadCooldown = ReloadTime;
        }

        if (isReloading)
        {
            moving = Vector2.Lerp(moving, new Vector2(0, 0), Time.deltaTime*10f);
            reloadCooldown -= Time.deltaTime;
            if (reloadCooldown < 0)
                isReloading = false;
        }
        else
        {
            moving = Vector2.Lerp(moving, new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")), Time.deltaTime*acceleration);

            if (Math.Abs(Input.GetAxis("Horizontal")) > 0.1f || Math.Abs(Input.GetAxis("Vertical")) > 0.1f)
                facing = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        }

        var measuredVelocity = (transform.position - lastUpdatePosition)/Time.deltaTime;
        //Debug.Log("Speed: " + measuredVelocity.magnitude);

        // Animation
        animatorThing.SetBool("Firing", Input.GetButton("Fire1"));
        animatorThing.SetBool("Reloading", isReloading);
        animatorThing.SetFloat("ForwardBackward", moving.y);
        animatorThing.SetFloat("LeftRight", moving.x);

        var aimAtPosition = GetScreenPointInWorldPlane(Input.mousePosition, 0f);
        var toAimPosition = aimAtPosition - transform.position;
        aimYaw = Quaternion.LookRotation(toAimPosition).eulerAngles.y;
        relativeAimYaw = Mathf.Lerp(relativeAimYaw, Mathf.DeltaAngle(transform.eulerAngles.y, aimYaw), Time.deltaTime*turnSpeed);

        var targetYaw = aimYaw;

        var velocity = Vector3.ClampMagnitude(new Vector3(moving.x, 0f, moving.y), 1f) * WalkSpeed;

        if (moving.magnitude > 0.1f)
            playerController.Move(velocity*Time.deltaTime);

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(transform.eulerAngles.x, targetYaw, transform.eulerAngles.z), turnSpeed*Time.deltaTime);

        ChaseCamera.transform.position = transform.position + new Vector3(0, 5f, -5f);
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