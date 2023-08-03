using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Suspension : MonoBehaviour
{
    private Rigidbody rb;

    [Header("Suspension")]
    [SerializeField] float restLength;
    [SerializeField] float springTravel;
    [SerializeField] float springStiffness;
    [SerializeField] float damperStiffness;

    private float minLength;
    private float maxLength;
    private float previousLength;
    private float springLength;
    private float springVelocity;
    private float springForce;
    private float damperForce;

    private Vector3 suspensionForce;

    [Header("Wheel")]
    [SerializeField] float wheelRadius;

    [Header ("Ground Information")]
    public bool touchingGround;
    public Vector3 groundNormal;
    void Awake()
    {
        rb = transform.root.GetComponent<Rigidbody>();

        minLength = restLength - springTravel;
        maxLength = restLength + springTravel;
    }

    void FixedUpdate()
    {
        if(Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, maxLength + wheelRadius))
        {
            touchingGround = true;
            groundNormal = hit.normal;

            previousLength = springLength;
            springLength = hit.distance - wheelRadius;
            springLength = Mathf.Clamp(springLength, minLength, maxLength);
            springVelocity = (previousLength - springLength) / Time.fixedDeltaTime;
            springForce = springStiffness * (restLength - springLength);
            damperForce = damperStiffness * springVelocity;

            suspensionForce = (springForce + damperForce) * transform.up;
            rb.AddForceAtPosition(suspensionForce, hit.point);
        }
        else
        {
            touchingGround = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(this.transform.position, this.transform.position + (restLength * -this.transform.up));
        Gizmos.color = Color.green;
        Gizmos.DrawLine(this.transform.position + (wheelRadius * -this.transform.up) + (restLength * -this.transform.up), this.transform.position + (restLength * -this.transform.up));

        Gizmos.color = Color.red;
        Gizmos.DrawLine(this.transform.position + (0.1f * -this.transform.forward), this.transform.position + (minLength * -this.transform.up) + (0.1f * -this.transform.forward));
        Gizmos.color = Color.green;
        Gizmos.DrawLine(this.transform.position + (wheelRadius * -this.transform.up) + (minLength * -this.transform.up) + (0.1f * -this.transform.forward), this.transform.position + (maxLength * -this.transform.up) + (0.1f * -this.transform.forward));

        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(this.transform.position + (0.1f * this.transform.forward), this.transform.position + (maxLength * -this.transform.up) + (0.1f * this.transform.forward));
        Gizmos.color = Color.green;
        Gizmos.DrawLine(this.transform.position + (wheelRadius * -this.transform.up) + (maxLength * -this.transform.up) + (0.1f * this.transform.forward), this.transform.position + (maxLength * -this.transform.up) + (0.1f * this.transform.forward));
    }
}
