using System;
using UnityEngine;

public class Dude : MonoBehaviour
{
    Animator animatorThing;
    public Camera ChaseCamera;
    public float ReloadTime;

    // Movement
    private Vector2 moving;
    private Vector2 facing;
    private float turnSpeed;
    
    // Reloading
    private bool isReloading;
    private float reloadCooldown;

    // Measure Speed
    private Vector3 lastUpdatePosition;
    private float lastUpdateTime;

    private CharacterController playerController;

    void Awake()
    {
        playerController = GetComponent<CharacterController>();
        animatorThing = GetComponent<Animator>();
        moving = new Vector2(0, 1f);
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
            moving = Vector2.Lerp(moving, new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")), Time.deltaTime*10f);

            if (Math.Abs(Input.GetAxis("Horizontal")) > 0.1f || Math.Abs(Input.GetAxis("Vertical")) > 0.1f)
                facing = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        }

        var measuredVelocity = (transform.position - lastUpdatePosition)/Time.deltaTime;
        //Debug.Log("Speed: " + measuredVelocity.magnitude);

        // Animation
        animatorThing.SetBool("Firing", Input.GetButton("Fire1"));
        animatorThing.SetBool("Reloading", isReloading);
        animatorThing.SetFloat("Speed", Vector3.ClampMagnitude(moving, 1f).magnitude);

        var targetYaw = Mathf.Atan2(facing.x, facing.y)*Mathf.Rad2Deg;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(transform.eulerAngles.x, targetYaw, transform.eulerAngles.z), turnSpeed*Time.deltaTime);

        var aimAtPosition = GetScreenPointInWorldPlane(Input.mousePosition, 0f);
        var toAimPosition = aimAtPosition - transform.position;
        var aimYaw = Quaternion.LookRotation(toAimPosition).eulerAngles.y;
        var relativeAimYaw = aimYaw - transform.eulerAngles.y;

        //Debug.Log("Yaw: " + relativeAimYaw);

        if (relativeAimYaw > 90 && relativeAimYaw < 270)
        {
            Debug.Log("Walk Backwards");
        }

        ChaseCamera.transform.position = transform.position + new Vector3(0, 7.5f, -7.5f);
        ChaseCamera.transform.LookAt(transform.position);

        lastUpdatePosition = transform.position;
        lastUpdateTime = Time.deltaTime;
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
