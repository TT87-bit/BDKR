using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PieChart.ViitorCloud;

public class ItemBank : MonoBehaviour
{
    [SerializeField] ItemProbabilty[] itemProbabilties;
    [SerializeField] PieChart.ViitorCloud.PieChart pc;

    void Awake()
    {
        for(int i = 0; i < itemProbabilties.Length; i++)
        {
            pc.Data[i] = itemProbabilties[i].weight;
        }
    }
}

[System.Serializable]
public class ItemProbabilty
{
    public string item;
    public float weight;
}