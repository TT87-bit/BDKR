using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character Stat Set", menuName = "Stat Thing/Character Stat Set")]
public class CharacterStatSet : StatSet
{
    [SerializeField] protected float fallChance;

    public new void OnEnable()
    {
        base.OnEnable();
        baseStatsTable.Add("fallChance", fallChance);
    }

}
