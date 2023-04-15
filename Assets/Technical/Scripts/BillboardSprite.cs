using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]

public class BillboardSprite : MonoBehaviour
{
    private Camera activeCamera;

    // Start is called before the first frame update
    void Start()
    {
        SetActiveCamera();
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.rotation = activeCamera.transform.rotation;
        this.transform.Rotate(Vector3.up , 180, Space.Self);
        this.transform.Rotate(Vector3.right, 90, Space.Self);
    }

    void SetActiveCamera()
    {
        activeCamera = Camera.main;
    }
}
