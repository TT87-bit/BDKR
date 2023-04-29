using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuspensionKartController : MonoBehaviour
{
    [Header ("Basic Stats")]
    [SerializeField] VehicleStatSet vehicleStats;
    [SerializeField] CharacterStatSet character1Stats;
    [SerializeField] CharacterStatSet character2Stats;
    CharacterStatSet config1Stats;
    CharacterStatSet config2Stats;
    CharacterStatSet activeConfigStats;
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
    float totalTraction; //The traction stat of the player's current configuration, times the average of all 0-1 traction values each grounded suspension object gets from the bit of floor they're over. Midair wheels will return 0 for this.
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

    /// <summary>
    /// Uses the current vehicle's frontCharInfluence stat to calculate base stats for each position the player can have the characters on the kart.
    /// </summary>
    void CalculateConfigStats()
    {
        float frontCharInfluence = vehicleStats.frontInfluence;
        float backCharInfluence = frontCharInfluence - 1;

        foreach(string stat in character1Stats.statNames)
        {
            if(Array.IndexOf(additiveStats, stat) != -1)
            {
                float value = character1Stats.GetStat(stat) + character2Stats.GetStat(stat) + vehicleStats.GetStat(stat);
                config1Stats.SetStat(stat, value);
                config2Stats.SetStat(stat, value);
            }
            else if(Array.IndexOf(frontOnlyStats, stat) != -1)
            {
                config1Stats.SetStat(stat, character1Stats.GetStat(stat));
                config2Stats.SetStat(stat, character2Stats.GetStat(stat));
            }
            else if(Array.IndexOf(backOnlyStats, stat) != -1)
            {
                config1Stats.SetStat(stat, character2Stats.GetStat(stat));
                config2Stats.SetStat(stat, character1Stats.GetStat(stat));
            }
            else
            {
                float value = (character1Stats.GetStat(stat) * frontCharInfluence + character2Stats.GetStat(stat) * backCharInfluence + vehicleStats.GetStat(stat)) / 2;
                config1Stats.SetStat(stat, value);
                value = (character2Stats.GetStat(stat) * frontCharInfluence + character1Stats.GetStat(stat) * backCharInfluence + vehicleStats.GetStat(stat)) / 2;
                config1Stats.SetStat(stat, value);
            }
        }
    }
}
