using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[CreateAssetMenu(fileName = "New Stat Modifier", menuName = "Stat Thing/Stat Modifier")]
public class StatModifier : ScriptableObject, IComparer<StatModifier>
{
    public float order;
    public bool considerOffRoad = false;
    public float duration;
    public Stat[] stats;

    public int Compare(StatModifier x, StatModifier y)
    {
        return Mathf.FloorToInt(x.order - y.order);
    }
}

[System.Serializable]
public class Stat
{
    public string stat;
    public float value = 1.0f;
    public bool overwriteGreater = false;
    public bool overwriteLesser = false;
}