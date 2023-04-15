using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Game Ruleset", menuName = "Game Ruleset")]
public class GameRuleset : ScriptableObject
{
    public EngineClass engineClass;
    public float cpuDifficulty;
}
