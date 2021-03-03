using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereCarController : MonoBehaviour
{
    public Rigidbody sphereCollider;

    private float horizontal;
    private float vertical;

    [Header("Turning Options")]
    [SerializeField]
    private float turnSpeed;
    [SerializeField]
    private float turnVelocity;
    [SerializeField]
    private float currAngle;
    [SerializeField]
    private float maxTurnAngle;

    [Header("Acceleration Options")]
    [SerializeField]
    private float accelerationRate;
    [SerializeField]
    private float currAcceleration;
    [SerializeField]
    private float maxAcceleration;

    [Header("Speed Options")]
    [SerializeField]
    private float currSpeed;
    [SerializeField]
    private float maxSpeed;

    [SerializeField]
    [Tooltip("Speed * Friction. A value of 0 means the car will not slow down.")]
    [Range(0.1f, 1f)]
    private float friction;

    private void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
    }

    void FixedUpdate()
    {
        //Follow Collider
        transform.position = sphereCollider.position;

        HandleMovement();
        HandleSteering();
    }

    void HandleSteering()
    {
        if(sphereCollider.velocity.magnitude <= 0.1f)
        {
            return;
        }
        float targetAngle = currAngle + (horizontal * maxTurnAngle);
        float angle = Mathf.SmoothDamp(transform.localEulerAngles.y, targetAngle, ref turnVelocity, turnSpeed);
        transform.localEulerAngles = new Vector3(0, angle, 0);
        currAngle = transform.localEulerAngles.y;
    }

    void HandleMovement()
    {
        if (vertical != 0)
        {
            currAcceleration += vertical * accelerationRate;
        }

        //Forward Acceleration
        sphereCollider.AddForce(transform.forward * currAcceleration);

        //Clamp Acceleration
        if(Mathf.Abs(currAcceleration) > maxAcceleration)
        {
            currAcceleration = maxAcceleration * vertical;
        }

        currSpeed = sphereCollider.velocity.magnitude;

        //Clamp Speed
        if(currSpeed >= maxSpeed)
        {
            sphereCollider.AddForce(transform.forward * -1 * currAcceleration);
        }
    }
}
