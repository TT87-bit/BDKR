using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapRenderer : MonoBehaviour
{
    [SerializeField] Sprite icon;
    [SerializeField] bool useRotation;
    private static GameObject map;
    [SerializeField] static float minX;
    [SerializeField] static float maxX;
    [SerializeField] static float minZ;
    [SerializeField] static float maxZ;
    [SerializeField] static float rotationOffset;
    [SerializeField] static Vector2 rotationPivot = new Vector2(0, 0);
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
