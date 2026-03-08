using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SubCharacterManager : MonoBehaviour
{
    public Image SubCharacterImage;

    public List<CharacterPortrait> SubCharacters;

    Dictionary<string, Sprite> _portraitDict;

    void Awake()
    {
        _portraitDict = new Dictionary<string, Sprite>();

        foreach (var c in SubCharacters)
        {
            _portraitDict[c.Name] = c.Portrait;
        }
    }

    public void ShowSubCharacter(string name)
    {
        if (_portraitDict.TryGetValue(name, out Sprite sprite))
        {
            SubCharacterImage.sprite = sprite;
        }
    }
}