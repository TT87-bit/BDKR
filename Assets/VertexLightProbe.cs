using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VertexLightProbe : MonoBehaviour
{
    [SerializeField] Material[] materials;
    public Color highlightColor, lowlightColor;
    public bool lightIsDirectional;
    public Vector3 lightPoint, lightDirection;
    [SerializeField] Transform directionCalculationPoint;
    public float updateSpeed;

    private void Awake()
    {
        UpdateMaterialList();
    }

    private void Update()
    {
        for (int i = 0; i < materials.Length; i++)
        {
            materials[i].SetColor("Highlight Color", highlightColor);
            materials[i].SetColor("Lowlight Color", lowlightColor);
            Vector3 calculatedLightDirection = lightIsDirectional ? lightDirection : Vector3.Normalize(directionCalculationPoint.position - lightPoint);
            materials[i].SetVector("Light Direction", calculatedLightDirection);
        }
    }

    public void UpdateMaterialList()
    {
        for (int i = 0; i < this.transform.childCount; i++)
        {
            materials[i] = GetComponentInChildren<MeshRenderer>().material;
        }
    }
}
