using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuspensionKartController : MonoBehaviour
{
    [Header ("Basic Stats")]
    [SerializeField] VehicleStatSet vehicleStats;
    [SerializeField] CharacterStatSet character1Stats;
    [SerializeField] CharacterStatSet character2Stats;
    StatSet config1Stats;
    StatSet config2Stats;
    StatSet activeConfigStats;
    float configTopSpeed;
    float configAcceleration;
    float configHandling;
    float configWeight;
    float configTraction;

    [Header ("Game-Wide Constants")]
    const float handlingToSteerAccelerationMultiplier = 0.5f; //The value multiplied by the kart's current handling stat to find how much the kart should rotate each frame on its way to match the player's steer input
    const float handlingToDriftWidthDividend = 2f;
    const float topSpeedToReverseTopSpeedMultiplier = 0.5f;
    const float accelerationToReverseAccelerationMultiplier = 0.5f;
    const float speedBonusPerToken = 2f;
    const float drag = 0.1f;
    readonly string[] additiveStats = {"weight"};
    readonly string[] frontOnlyStats;
    readonly string[] backOnlyStats;

    [Header ("Suspension Stuff")]
    [SerializeField] GameObject[] suspensionObjects;
    Suspension[] suspensionScripts;

    [Header ("Movement/Physics Calculation")]
    float wheelPush = 0f; //How far the wheels are "trying" to push the kart forward, before grip is taken into account; purely based on how fast they'd be turning. Negative value means the kart is trying to reverse.
    float totalTraction; //The traction stat of the player's current configuration, times the average of all 0-1 traction values each grounded suspension object gets from the bit of floor they're over.
    float grippedPush = 0f; //wheelPush * totalTraction.
    float currentSteer = 0f;
    float goalSteer = 0f; //Current horizontal turn input * the vehicle's current handling stat. What currentSteer approaches by steerAcceleration units each frame
    float steerRotation = 0f;
    float driftWidth = 0f;
    float driftRotation = 0f;
    Vector2 velocityFromWheels; //Oriented to be tangent to the ground normals from the suspension scripts, and so that the Y axis points forward
    Vector2 previousVelocityFromWheels;
    Vector3 worldVelocity;
    Vector3 previousWorldVelocity;

    [Header ("Other")]
    [SerializeField] float swtichSpeed;
    float switchTimer;

    private void Awake() 
    {
        CalculateConfigStats();

        for(int i = 0; i < suspensionObjects.Length; i++)
        {
            suspensionScripts[i] = suspensionObjects[i].GetComponent<Suspension>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate() 
    {
        //previousVelocityFromWheels = velocityFromWheels;
        //previousWorldVelocity = worldVelocity;
        //if accelerate button is being pressed, wheelPush += configAcceleration * deltaTime;
        //if brake/reverse button is being pressed, wheelPush -= configAcceleration * deltaTime;
        //while both buttons are being pressed, and the absolute value of 0 - wheelPush is less than a threshold, increase the player's handling because they're trying to Pivot Steer
        //clamp wheelPush to be between -configTopSpeed * topSpeedToReverseTopSpeedMultiplier and configTopSpeed
        //grippedPush = wheelPush * totalTraction;

        //if currentSteer is less than goalSteer, set currentSteer to max(goalSteer, currentSteer - steer acceleration * deltaTime)
        //if currentSteer is greater than goalSteer, set currentSteer to min(goalSteer, currentSteer + steer acceleration * deltaTime)
        //if all the front wheels are grounded, set steerRotation to currentSteer * deltaTime;

        //if not drifting
        //  velocityFromWheels = new Vector2(0, grippedPush);
        //  driftRotation = 0;

        //if drifting
        //  driftWidth = handlingToDriftWidthDividend / kart's current handling
        //  driftWidth /= totalTraction
        //  velocityFromWheels = new Vector2(driftWidth, 1);
        //  velocityFromWheels = grippedPush * velocityFromWheels.normalized
        //  driftRotation = however many degrees of rotation it'd take to make the new velocityFromWheels vector point straight out the kart's side
        
        //velocityFromWheels = lerp(previousVelocityFromWheels, velocityFromWheels, totalTraction);
        //worldVelocity = new Vector3(velocityFromWheels oriented as specified above);
        //worldVelocity *= 1 - drag;
        
    }

    void CalculateConfigStats()
    {
        float frontCharInfluence = vehicleStats.frontInfluence;
        float backCharInfluence = frontCharInfluence - 1;

        float[] character1StatsArray = character1Stats.Stats2Array();
        float[] character2StatsArray = character2Stats.Stats2Array();
        float[] vehicleStatsArray = vehicleStats.Stats2Array();

        float[] config1StatsArray = {0};
        for(int i = 0; i < character1StatsArray.Length; i++)
        {
            config1StatsArray[i] = (character1StatsArray[i] * frontCharInfluence + character2StatsArray[i] * backCharInfluence + vehicleStatsArray[i]) / 2;
        }
        config1Stats.array2Stats(character1StatsArray);

        float[] config2StatsArray = {0};
        for(int i = 0; i < character1StatsArray.Length; i++)
        {
            config2StatsArray[i] = (character2StatsArray[i] * frontCharInfluence + character1StatsArray[i] * backCharInfluence + vehicleStatsArray[i]) / 2;
        }
        config2Stats.array2Stats(character2StatsArray);
    }

    
}
