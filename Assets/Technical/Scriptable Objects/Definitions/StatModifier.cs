using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[CreateAssetMenu(fileName = "New Stat Modifier", menuName = "Stat Thing/Stat Modifier")]
public class StatModifier : ScriptableObject, IComparer<StatModifier>
{
    public float order;
    public bool fromOffRoad = false;
    public float duration;
    [SerializeField] Stat[] stats;

    public int Compare(StatModifier x, StatModifier y)
    {
        return Mathf.FloorToInt(x.order - y.order);
    }
}

[System.Serializable]
public class Stat
{
    public string name;
    public float value;
    public bool overwriteGreater;
    public bool overwriteLesser;
}