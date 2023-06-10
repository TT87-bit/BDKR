using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KartModifier : MonoBehaviour
{
    SortedList<float, StatModifier> modifierList = new SortedList<float, StatModifier>();
    SuspensionKartController kartController;
    private void Awake() 
    {
        kartController = this.GetComponent<SuspensionKartController>();
        RefreshList();
    }

    public void Apply(StatModifier modifier)
    {
        Apply(modifier, modifier.duration);
    }

    // Add the modifier to the list
    public void Apply(StatModifier modifier, float duration)
    {
        // Make a duplicate modifier
        StatModifier mod = Instantiate(modifier) as StatModifier;

        // Change the duplicate's time
        mod.duration = duration;

        // Add the duplicate to the list
        modifierList.Add(mod.order, mod);

        // Refresh the list
        RefreshList();
    }

    public void Clear()
    {
        
        RefreshList();
    }
    
    // Updates the list so that the modifiers are in order of their priority value

    //When actually applying modifiers to kart, we'll go down the list (thinking hashtable or heap) of StatModifiers, and for each stat
    //in each modifier, we either multiply it to the current stat lineup at that point or overwrite it with that modifier's value for the
    //stat
    public void RefreshList()
    {
        CharacterStatSet outputStats = kartController.activeConfigStats;
        outputStats.SetStat("offroad", 0);
        print(outputStats.GetStat("offroad") + " / " + kartController.activeConfigStats.GetStat("offroad"));
        foreach(StatModifier modifier in modifierList.Values)
        {

        }
    }
}
