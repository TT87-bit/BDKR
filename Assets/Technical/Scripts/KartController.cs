using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEditor;

[SerializeField] class KartController : MonoBehaviour
{
    Rigidbody rb;
    [Header ("Vehicle Stats")]
    [SerializeField] float acceleration = 50;
    [SerializeField] float traction = 1;
    [SerializeField] float topSpeed = 30, topReverseSpeed = 10;
    [SerializeField] float accelerationSlipSpeed = 20f;
    [SerializeField] float rollingFriction = 0.98f;
    [SerializeField] float steerAngle = 20;
    [SerializeField] Transform steerAxis;
    [SerializeField] float speedForFullTurn = 10;
    float reverseTurningMultiplier = 0.5f;
    private Vector3 velocity, reverseVelocity;
    Vector3 totalVelocity {get => velocity + reverseVelocity + (Vector3.down * currentGravityVelocity);}
    [SerializeField] float speed {get => velocity.magnitude - reverseVelocity.magnitude;}

    [Header ("Midair Handling")]
    [SerializeField] Transform groundCheck;
    public bool isGrounded {get; private set;}
    bool wasGrounded = false;
    float airTime = 0;
    [SerializeField] float airSteerPeriod = 0.5f;
    AlignWithGround groundAligner;
    
    [Header ("Gravity")]
    [SerializeField] float gravityAcceleration = 0.5f;
    public float currentGravityVelocity {get; private set;}
    private float steerInput;
    private float currentAngle = 0;
    private bool gasInput, reverseInput;

    private void Awake() 
    {
        rb = this.GetComponent<Rigidbody>();
        groundAligner = this.GetComponent<AlignWithGround>();
    }
    
    void Update()
    {
        Move();
        Steer();
        ApplyTraction();
        HandleMidAir();
        ApplyGravity();
    }

    private void FixedUpdate() 
    {
        rb.velocity = totalVelocity;
    }
    
    private void LateUpdate() 
    {
        float yRotation = currentAngle - transform.localEulerAngles.y;
        this.transform.Rotate(0, yRotation, 0, Space.Self);
    }

    void HandleMidAir() 
    {
        isGrounded = Physics.OverlapBox(groundCheck.position, groundCheck.localScale, this.transform.rotation, LayerMask.GetMask("Ground")).Length > 0;
        // print("wasGrounded: " + wasGrounded + " | isGrounded: " + isGrounded + " | airTime: " + airTime);
        if (!wasGrounded && isGrounded)
        {
            // print("hit floor");
            // if we were in air and this collision can ground us
            if (airTime > 0.05f)
            {
                // print("grounding");
                // // project our velocity onto the new plane
                // print(velocity);
                // print(groundAligner.averageNormal);
                // velocity = Vector3.ProjectOnPlane(velocity, groundAligner.averageNormal);
                // // print(velocity);
                // reverseVelocity = Vector3.ProjectOnPlane(reverseVelocity, groundAligner.averageNormal);
            }
        }
        // are we grounded
        // if were not grounded, add to the in air timer, if that timer is over a certain value the steering wont apply 
        if (!isGrounded)
        {
            // add to the in air timer
            airTime += Time.deltaTime;
        }
        // if we are grounded and werent before, reset the in air timer and project velocity vectors onto new plane
        else
        {
            airTime = 0;
            velocity = Vector3.ProjectOnPlane(velocity, groundAligner.averageNormal);
            // print(velocity);
            reverseVelocity = Vector3.ProjectOnPlane(reverseVelocity, groundAligner.averageNormal);
        }

        wasGrounded = isGrounded;
    }   


    void ApplyGravity() 
    {
        if(!isGrounded)
        {
            currentGravityVelocity += gravityAcceleration * Time.fixedDeltaTime;
        }
        else
        {
            currentGravityVelocity = 0;
        }
    }

    Vector3 accelerationDirection;

    void Move() 
    {
        // Accelerating
        accelerationDirection = speed > accelerationSlipSpeed ? velocity.normalized : transform.forward;
        velocity += accelerationDirection * acceleration * (gasInput && isGrounded ? 1 : 0) * Time.deltaTime;
        
        // Reversing
        reverseVelocity += -transform.forward * acceleration * (reverseInput && isGrounded ? 1 : 0) * Time.deltaTime;

        // Friction and top speed
        velocity *= !gasInput && isGrounded ? rollingFriction : 1; // Only applies rolling friction it when the gas is not pressed
        velocity = Vector3.ClampMagnitude(velocity, topSpeed);

        reverseVelocity *= !reverseInput && isGrounded ? rollingFriction : 1;
        reverseVelocity = Vector3.ClampMagnitude(reverseVelocity, topReverseSpeed);
    }

    void Steer()
    {
        float speedMultiplier = speed >= speedForFullTurn ? 1 : speed / speedForFullTurn;
        float reverseMultiplier = speed < 0 ? reverseTurningMultiplier : 1;
        bool canSteer = airTime < airSteerPeriod;
        if (canSteer)
        {
            currentAngle += steerInput * speedMultiplier * reverseMultiplier * steerAngle * Time.deltaTime; // Makes steer input no logner affect turning after its above 1
        }
    }

    void ApplyTraction()
    {
        Debug.DrawRay(transform.position, velocity.normalized * 3, Color.red);
        Debug.DrawRay(transform.position, transform.forward * 3, Color.blue);
        if(isGrounded)
        {
            velocity = Vector3.Lerp(velocity.normalized, transform.forward, traction * Time.deltaTime) * velocity.magnitude;
        }
    }
    
    
#region Input
    public void OnSteer(InputAction.CallbackContext context)
    {
        steerInput = context.ReadValue<float>();
    }

    public void OnGas(InputAction.CallbackContext context)
    {
        gasInput = context.ReadValue<float>() > 0;
    }

    public void OnReverse(InputAction.CallbackContext context)
    {
        reverseInput = context.ReadValue<float>() > 0;
    }
#endregion

    private void OnDrawGizmos() 
    {
        if(!EditorApplication.isPlaying)
        {
            return;
        }
            
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(this.transform.position, this.transform.position + accelerationDirection * 5);
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(this.transform.position, this.transform.position + velocity);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(this.transform.position, this.transform.position + rb.velocity);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(this.transform.position, this.transform.position + Vector3.down * currentGravityVelocity);
    }
}
