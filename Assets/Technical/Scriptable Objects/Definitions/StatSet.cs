using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Stat Set", menuName = "Stat Thing/Stat Set")]
public class StatSet : ScriptableObject
{
    [SerializeField] protected float topSpeed = 0;
    [SerializeField] protected float acceleration = 0;
    [SerializeField] protected float handling = 0;
    [SerializeField] protected float weight = 0;
    [SerializeField] protected float traction = 0;
    [SerializeField] protected float offroad = 0;
    protected Hashtable baseStats;
    public string[] statNames
    {
        get
        {
            string[] stats = new string[baseStats.Count];
            baseStats.Keys.CopyTo(stats, 0);
            return stats;
        }
    }

    protected void OnEnable() {
        baseStats = new Hashtable{
            {"topSpeed", topSpeed},
            {"acceleration", acceleration},
            {"handling", handling},
            {"weight", weight},
            {"traction", traction},
            {"offroad", offroad}
        };
    }

    public void SetStat(string name, float value)
    {
        if(!baseStats.ContainsKey(name))
        {
            return;
        }

        baseStats[name] = value;
    }

    public float GetStat(string name)
    {
        if(!baseStats.ContainsKey(name))
        {
            return -1;
        }

        return (float)baseStats[name];
    }

    Hashtable generateStatTable(ICollection collection)
    {
        Hashtable output = new Hashtable()
        {
            {"topSpeed", 0}, {"acceleration", 0}, {"handling", 0}, {"weight", 0}, {"traction", 0}
        };

        return output;
    }

    // datas
    //HashMap<string, float> stats = new HashMap<string, float>();

    ArrayList modifiers = new ArrayList();

    //Apply() {
        // for each modifier

            // modify the karts stats based on modifier


        // after a certain amount of time call clear
        
        //
    //}

    //things to
    //flag that says whether to take player's offroad stat into account when applying
    //for each stat in a Stat Set:
    //  a float value
    //  3 bools determining whether the value should be multiplied with the player's stats, overwrite lower values, or overwrite higher values
}
