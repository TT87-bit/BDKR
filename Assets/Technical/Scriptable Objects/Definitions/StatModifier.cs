using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[CreateAssetMenu(fileName = "New Stat Modifier", menuName = "Stat Thing/Stat Modifier")]
public class StatModifier : ScriptableObject
{
    public float order;
    public float overwriteGreater;
    public float overwriteLesser;
    public bool fromOffRoad = false;
    [SerializeField] Stat[] stats;
}

[System.Serializable]
public class Stat
{
    public string name;
    public float value;
}

public enum statNames
{
    //topSpeed = "Top Speed, "
}