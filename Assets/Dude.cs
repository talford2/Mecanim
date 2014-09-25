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
    private float aimYaw;
    private float relativeAimYaw;

    private Transform spine1;
    
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

        spine1 = transform.FindChild("Hips").FindChild("Spine").FindChild("Spine1").FindChild("Spine2");
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

        var aimAtPosition = GetScreenPointInWorldPlane(Input.mousePosition, 0f);
        var toAimPosition = aimAtPosition - transform.position;
        aimYaw = Quaternion.LookRotation(toAimPosition).eulerAngles.y;
        relativeAimYaw = aimYaw - transform.eulerAngles.y;

        var targetYaw = Mathf.Atan2(facing.x, facing.y)*Mathf.Rad2Deg;

        if (relativeAimYaw > 90 && relativeAimYaw < 270)
        {
            Debug.Log("Walk Backwards");
            //targetYaw = aimYaw;
        }

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(transform.eulerAngles.x, targetYaw, transform.eulerAngles.z), turnSpeed * Time.deltaTime);

        ChaseCamera.transform.position = transform.position + new Vector3(0, 5f, -5f);
        ChaseCamera.transform.LookAt(transform.position);

        lastUpdatePosition = transform.position;
        lastUpdateTime = Time.deltaTime;
    }

    private void LateUpdate()
    {
        //spine1.localRotation = Quaternion.AngleAxis(aimYaw, Vector3.forward);
        spine1.rotation = Quaternion.Euler(spine1.eulerAngles.x, aimYaw+60f, spine1.eulerAngles.z);
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
