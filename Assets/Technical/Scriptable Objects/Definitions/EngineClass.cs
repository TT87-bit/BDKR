using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Engine Class", menuName = "EngineClass")]
public class EngineClass : ScriptableObject
{
    public new string name;
    public float Multiplier;
    public Sprite Icon;
}
