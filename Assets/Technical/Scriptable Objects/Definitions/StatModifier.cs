using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[CreateAssetMenu(fileName = "New Stat Modifier", menuName = "Stat Thing/Stat Modifier")]
public class StatModifier : ScriptableObject
{
    public float order;
    public bool overwriteGreater;
    public bool overwriteLesser;
    public bool fromOffRoad = false;
    public float duration;
    [SerializeField] Stat[] stats;

    public StatModifier Instantiate() {
        //Create copy of instance
        StatModifier instance = (StatModifier)ScriptableObject.CreateInstance(nameof(StatModifier));
        Debug.Log(instance.duration);
        return instance;
    }
}

[System.Serializable]
public class Stat
{
    public string name;
    public float value;
}