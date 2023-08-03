using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollingIntCounter : MonoBehaviour
{
    public int trueValue = 0;
    public int displayValue;
    public float incrementsPerSecond = 4;
    public float amountPerIncrement = 2;

    [Header ("Curves")]
    public bool useCurves;
    public AnimationCurve incrementsPerSecondGraph;
    public AnimationCurve amountPerIncrementGraph;
    public bool updateSpeedEachFrame;
    public bool useSameValuesForIncreasingAndDecreasing;


    int previousTrueValue;
    int displayToTrueDist;

    private void Awake() 
    {
        previousTrueValue = trueValue;
    }

    void Update()
    {
        if(useCurves && (updateSpeedEachFrame || trueValue != previousTrueValue))
        {
            UpdateSpeed();
        }

        {
            if(Mathf.Abs(displayToTrueDist) > (int)(Mathf.RoundToInt(amountPerIncrement * incrementsPerSecond * Time.deltaTime)))
            {
                displayValue = trueValue;
            }
            else
            {
                displayValue += (int)(Mathf.RoundToInt(amountPerIncrement * incrementsPerSecond * Time.deltaTime) * Mathf.Sign(displayToTrueDist));
            }
        }

        previousTrueValue = trueValue;
    }

    void UpdateSpeed()
    {
        displayToTrueDist = displayValue - trueValue;
        if(useSameValuesForIncreasingAndDecreasing)
        {
            displayToTrueDist = Mathf.Abs(displayToTrueDist);
        }

        incrementsPerSecond = incrementsPerSecondGraph.Evaluate(displayToTrueDist);
        amountPerIncrement = amountPerIncrementGraph.Evaluate(displayToTrueDist);
    }
}
