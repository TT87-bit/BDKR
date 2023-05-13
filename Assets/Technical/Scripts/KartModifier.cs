using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KartModifier : MonoBehaviour
{
    List<StatModifier> modifierList = new List<StatModifier>();
    KartController kartController;
    [SerializeField] StatModifier testModifier;
    [SerializeField] float testDuration;
    private void Awake() 
    {
        kartController = this.GetComponent<KartController>();
    }

    void Start()
    {
        testModifier.Instantiate();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Apply(StatModifier modifier)
    {
        Apply(modifier, modifier.duration);
    }

    // Add the modifier to the list
    public void Apply(StatModifier modifier, float duration)
    {
        // Make a duplicate modifier
        

        // Change the duplicate's time


        // Add the duplicate to the list


        // Refresh the list
        RefreshList();
    }

    public void Clear()
    {
        
        RefreshList();
    }
    
    public void RefreshList()
    {

    }
}
