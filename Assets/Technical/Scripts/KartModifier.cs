using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KartModifier : MonoBehaviour
{
    SortedList<float, StatModifier> modifierList = new SortedList<float, StatModifier>();
    SuspensionKartController kartController = new SuspensionKartController();
    private void Awake() 
    {
        kartController = this.GetComponent<SuspensionKartController>();
        RefreshModifiers();
    }

    //Have each modifier's timers count down by the amount of time passed since last frame.
    private void Update() 
    {
        foreach(KeyValuePair<float, StatModifier> modifier in modifierList)
        {
            modifier.Value.duration -= Time.deltaTime;
        }
    }

    //Remove any modifiers whose timers have counted down to 0. Done in LateUpdate so modifiers that started with a duration of zero could still work, to provide a simple way tp create modifiers that go away as soon as they're not being actively applied by the enviroment/terrain.
    private void LateUpdate() 
    {
        foreach(KeyValuePair<float, StatModifier> modifier in modifierList)
        {
            if(modifier.Value.duration <= 0.0)
            {
                modifierList.Remove(modifier.Key);
                RefreshModifiers();
            }
        }
    }

    public void Apply(StatModifier modifier)
    {
        Apply(modifier, modifier.duration);
    }

    // Adds the modifier to the list. If the modifier is already in the list, refresh the timer
    public void Apply(StatModifier modifier, float duration)
    {
        foreach(KeyValuePair<float, StatModifier> listModifier in modifierList)
        {
            if(modifier = listModifier.Value)
            {
                listModifier.Value.duration = duration;
                return;
            }
        }

        // Make a duplicate modifier
        StatModifier mod = Instantiate(modifier) as StatModifier;

        // Change the duplicate's time
        mod.duration = duration;

        // Add the duplicate to the list
        modifierList.Add(mod.order, mod);

        // Refresh the list
        RefreshModifiers();
    }

    public void Remove(StatModifier modifier)
    {
        foreach(KeyValuePair<float, StatModifier> listModifier in modifierList)
        {
            if(modifier = listModifier.Value)
            {
                modifierList.Remove(listModifier.Key);
                RefreshModifiers();
            }
        }
    }
    
    // Updates the list so that the modifiers are in order of their priority value

    //When actually applying modifiers to kart, we'll go down the list (thinking hashtable or heap) of StatModifiers, and for each stat
    //in each modifier, we either multiply it to the current stat lineup at that point or overwrite it with that modifier's value for the
    //stat
    public void RefreshModifiers()
    {
        CharacterStatSet outputStats = new CharacterStatSet();
        //CharacterStatSet outputStats = new ScriptableObject.CreateInstance(CharacterStatSet());
        outputStats = kartController.activeConfigStats;
    
        foreach(StatModifier modifier in modifierList.Values)
        {
            foreach(Stat stat in modifier.stats)
            {
                float baseValue = (float)outputStats.baseStats[stat.name];

                if(stat.overwriteGreater && stat.value > baseValue)
                {
                    baseValue = stat.value;
                }
                else if(stat.overwriteLesser && stat.value < baseValue)
                {
                    baseValue = stat.value;
                }
                else
                {
                    float fullOffRoadValue = baseValue * stat.value;
                    baseValue = modifier.considerOffRoad ? fullOffRoadValue : Mathf.Lerp(fullOffRoadValue, baseValue, (float)outputStats.baseStats["offroad"]);
                }

                outputStats.baseStats[stat.name] = baseValue;
            }
        }
    }
}
