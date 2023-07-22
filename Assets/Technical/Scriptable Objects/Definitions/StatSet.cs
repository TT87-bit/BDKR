using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Stat Set", menuName = "Stat Thing/Stat Set")]
public class StatSet : ScriptableObject
{
    [SerializeField] public float topSpeed = 0;
    [SerializeField] public float acceleration = 0;
    [SerializeField] public float handling = 0;
    [SerializeField] public float weight = 0;
    [SerializeField] public float traction = 0;
    [SerializeField] public float offroad = 0;
    public Hashtable baseStatsTable; // Legacy code using the hashtable, do not use for new code
    public float[] baseStats;   // New approach using array
    public string[] statNames
    {
        get
        {
            string[] stats = new string[baseStatsTable.Count];
            baseStatsTable.Keys.CopyTo(stats, 0);
            return stats;
        }
    }

    public void OnEnable() {
        // Legacy code using the hashtable, do not use for new code    
        baseStatsTable = new Hashtable{
            {"topSpeed", this.topSpeed},
            {"acceleration", this.acceleration},
            {"handling", this.handling},
            {"weight", this.weight},
            {"traction", this.traction},
            {"offroad", this.offroad}
        };

        baseStats = new float[] {this.topSpeed, this.acceleration, this.handling, this.weight, this.traction, this.offroad};
    }

    public void SetStat(string name, float value)
    {
        if(!baseStatsTable.ContainsKey(name))
        {
            return;
        }

        baseStatsTable[name] = value;
    }

    public float GetStat(string name)
    {
        if(!baseStatsTable.ContainsKey(name))
        {
            return -1;
        }

        return (float)baseStatsTable[name];
    }

    Hashtable generateStatTable(ICollection collection)
    {
        Hashtable output = new Hashtable()
        {
            {"topSpeed", 0}, {"acceleration", 0}, {"handling", 0}, {"weight", 0}, {"traction", 0}
        };

        return output;
    }
}
