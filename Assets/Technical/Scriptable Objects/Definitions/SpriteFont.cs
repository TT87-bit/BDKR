using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Sprite Font", menuName = "Sprite Font")]
public class SpriteFont : ScriptableObject
{
    public SpriteCharacter[] characters;
}

[System.Serializable]
public class SpriteCharacter
{
    public char character;
    public Sprite sprite;
}