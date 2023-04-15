using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Vehicle Stat Set", menuName = "Stat Thing/Vehicle Stat Set")]
public class VehicleStatSet : StatSet
{
    public float frontInfluence;

    new protected void OnEnable() 
    {
        base.OnEnable();
        baseStats.Add("frontInfluence", 0.0);
    }
}
