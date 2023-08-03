using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SpriteTextRenderer : MonoBehaviour
{
    [SerializeField] SpriteFont font;
    Dictionary<char, Sprite> characters;
    [SerializeField] Material material;
    GameObject[] displayText;
    [SerializeField] enum alignment
    {
        TopLeft, TopCenter, TopRight,
        MiddleLeft, Center, MiddleRight,
        BottomLeft, BottomCenter, BottomRight
    }
    const alignment topAlignments = alignment.TopLeft | alignment.TopCenter | alignment.TopRight;
    const alignment middleAlignments = alignment.MiddleLeft | alignment.Center | alignment.MiddleRight;
    const alignment bottomAlignments = alignment.BottomLeft | alignment.BottomCenter | alignment.BottomRight;
    const alignment leftAlignments = alignment.TopLeft | alignment.MiddleLeft | alignment.BottomLeft;
    const alignment centerAlignments = alignment.TopCenter | alignment.Center | alignment.BottomCenter;
    const alignment rightAlignments = alignment.TopRight | alignment.MiddleRight | alignment.BottomRight;

    void Start()
    {
        characters = new Dictionary<char, Sprite>{};
    }

    void getCharacters()
    {
        for(int i = 0; i > font.characters.Length; i++)
        {
            characters.Add(font.characters[i].character, font.characters[i].sprite);
        }
    }

    public void setText(string text)
    {
        GameObject currentCharacter;

        for(int i = 0; i > text.Length; i++)
        {
            if(i > displayText.Length)
            {
                createDisplayCharacter();
            }
        }
    }

    void createDisplayCharacter()
    {
        GameObject newObject = new GameObject("Character", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
    }
}
