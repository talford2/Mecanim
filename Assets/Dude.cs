using UnityEngine;

public class Dude : MonoBehaviour
{
    public Camera ChaseCamera;
    public float RunSpeed;
    public float WalkSpeed;
    public float ReloadTime;

    // Movement
    private Vector3 moving;
    private float acceleration;
    private Vector3 velocity;
    private float turnSpeed;
    private float aimYaw;
    private float relativeAimYaw;
    private CharacterController playerController;

    // Reloading
    private bool isReloading;
    private float reloadCooldown;

    // Animation
    private Animator animatorThing;

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
            moving = Vector3.Lerp(moving, new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical")), Time.deltaTime*acceleration);
        }

        var aimAtPosition = GetScreenPointInWorldPlane(Input.mousePosition, 0f);
        var toAimPosition = aimAtPosition - transform.position;
        aimYaw = Quaternion.LookRotation(toAimPosition).eulerAngles.y;

        var relativeToWalkYaw = Mathf.DeltaAngle(Mathf.Atan2(moving.x, moving.z) * Mathf.Rad2Deg, aimYaw) * Mathf.Deg2Rad;
        var forwardBackward = Mathf.Cos(relativeToWalkYaw);
        var leftRight = -Mathf.Sin(relativeToWalkYaw);
        var targetYaw = aimYaw;

        if (forwardBackward > 0f)
        {
            // Run Forward
            velocity = Vector3.Lerp(velocity, Vector3.ClampMagnitude(moving, 1f)*RunSpeed, Time.deltaTime*acceleration);
        }
        else
        {
            // Walk Backward
            velocity = Vector3.Lerp(velocity, Vector3.ClampMagnitude(moving, 1f)*WalkSpeed, Time.deltaTime*acceleration);
        }

        if (moving.magnitude > 0.1f)
            playerController.Move(velocity*Time.deltaTime);

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(transform.eulerAngles.x, targetYaw, transform.eulerAngles.z), turnSpeed*Time.deltaTime);

        // Animation
        animatorThing.SetFloat("Speed", moving.magnitude);
        animatorThing.SetFloat("ForwardBackward", forwardBackward);
        animatorThing.SetFloat("LeftRight", leftRight);

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