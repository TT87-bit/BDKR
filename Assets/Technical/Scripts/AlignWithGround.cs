using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This script is used to align the kart with the ground. 
It uses a multiple raycasts to detect the ground and then aligns the kart with the average normal of the raycasts. */
public class AlignWithGround : MonoBehaviour
{    
    [SerializeField] float raycastDistance = 1;
    [SerializeField] LayerMask ground;
    [SerializeField] Quaternion goalRotation;
    [SerializeField] float airAngleUpdateSpeed, groundAngleUpdateSpeed;
    KartController controller;
    float angleUpdateSpeed {get => controller.isGrounded ? groundAngleUpdateSpeed : airAngleUpdateSpeed;}
    float downVelocity {get => controller.currentGravityVelocity;}
    
    [Header ("Raycast Grid Generation")]
    [SerializeField] Vector3 center;
    [SerializeField] float zSpan;
    [SerializeField] float frontSpan;
    [SerializeField] float backSpan;
    [SerializeField] int length;
    [SerializeField] int width;
    public float rowGap {get => zSpan / (length - 1);}

    public Vector3 averageNormal {get; private set;}
    [SerializeField] float groundSnapRange = 1f;
    [SerializeField] float snapThreshold = 0.25f;
    Vector3 hitPos = Vector3.zero;

    private void Awake() 
    {
        controller = this.GetComponent<KartController>();
    }
    
    void Update()
    {
        // Make a box cast
        RaycastHit hit;
        Vector3 boxCastExtents = new Vector3((frontSpan + backSpan) / 4, 1, zSpan / 2);
        if(Physics.BoxCast(this.transform.position + center, boxCastExtents, -this.transform.up, out hit, this.transform.rotation, groundSnapRange * Mathf.Abs(downVelocity), ground))
        {
            Vector3 localHitPos = this.transform.InverseTransformPoint(hit.point);
            float distance = localHitPos.y;
            print(hit.transform.name);
            // Get the distance on the local y axis between the kart and the hit point and traslate the kart by that distance
            print("Hit distance: " + distance);
            if(Mathf.Abs(distance) < snapThreshold) {
                this.transform.Translate(Vector3.up * distance, Space.Self);
                print("Snap distance: " + (this.transform.InverseTransformPoint(this.transform.position).y));
            }
        }

        // If the box cast hits something, snap the kart positionally to the surface of the object

    }

    private void LateUpdate() 
    {
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, GetGroundOrientation(), angleUpdateSpeed * Time.deltaTime);
    }

    Quaternion GetGroundOrientation()
    {
        // averageNormal = Vector3.zero;
        Vector3 newNormal = Vector3.zero;
        Vector3[] raycastPositions = GenerateRaycastOrigins();
        foreach(Vector3 raycast in raycastPositions)
        {
            RaycastHit hit;
            if(Physics.Raycast(raycast, Vector3.down, out hit, raycastDistance, ground))
            {
                newNormal += hit.normal;
            }
        }

        averageNormal = newNormal.normalized;
        return Quaternion.FromToRotation(Vector3.up, averageNormal);
    }

    Vector3[] GenerateRaycastOrigins() 
    {
        Vector3[] points = new Vector3[width * length];
        for (int row = 0; row < length; row++)
        {
            float zOffset = (row * rowGap) - (zSpan / 2);
            float rowWidth = Mathf.Lerp(backSpan, frontSpan, (float)row / (float)(length - 1));
            float columnGap = rowWidth / (width - 1);
            for(int column = 0; column < width; column++)
            {
                float xOffset = (column * columnGap) - (rowWidth / 2);
                Vector3 pointOffset = this.transform.rotation * new Vector3(xOffset, 0, zOffset);
                points[(row * width) + column] = this.transform.position + center + pointOffset;
            }
        }
        return points;
    }

    void OnDrawGizmos() 
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(this.transform.position, this.transform.position + (averageNormal * 5));

        // Generate lines for each individual raycast, raycasts that arent hitting anything are drawn in red those that are hitting are drawn in blue
        Vector3[] raycastPositions = GenerateRaycastOrigins();
        foreach(Vector3 raycast in raycastPositions)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(raycast, raycast + (Vector3.down * raycastDistance));
            RaycastHit hit;
            if(Physics.Raycast(raycast, Vector3.down, out hit, raycastDistance, ground))
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(raycast, hit.point);
            }
        }

        Gizmos.DrawSphere(hitPos, 0.5f);
    }
}
