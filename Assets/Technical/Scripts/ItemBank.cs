using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBank : MonoBehaviour
{
    [SerializeField] ItemProbabilty[] itemProbabilties;
}

[System.Serializable]
public class ItemProbabilty
{
    public string item;
    public float weight;
}