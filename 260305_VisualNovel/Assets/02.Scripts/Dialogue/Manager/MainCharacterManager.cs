using UnityEngine;
using System.Collections.Generic;

public class MainCharacterManager : MonoBehaviour
{
    public SpriteRenderer MainCharacterRenderer;
    public List<MainCharacter> Characters;

    private Dictionary<string, Dictionary<string, Sprite>> _characterDict;

    private static readonly Color ActiveColor = new Color(1f, 1f, 1f, 1f);
    private static readonly Color DimColor = new Color(0.69f, 0.69f, 0.69f, 1f);

    void Awake()
    {
        _characterDict = new Dictionary<string, Dictionary<string, Sprite>>();
        foreach (var character in Characters)
        {
            var expressionDict = new Dictionary<string, Sprite>();
            foreach (var exp in character.Expressions)
                expressionDict[exp.Expression] = exp.Portrait;
            _characterDict[character.CharacterName] = expressionDict;
        }
    }

    public void SetCharacterSprite(string characterName, string expression)
    {
        if (string.IsNullOrEmpty(characterName) || string.IsNullOrEmpty(expression)) return;
        if (!_characterDict.TryGetValue(characterName, out var expDict)) return;
        if (!expDict.TryGetValue(expression, out var sprite)) return;
        MainCharacterRenderer.sprite = sprite;
    }

    public void SetActive() => MainCharacterRenderer.color = ActiveColor;
    public void SetDim() => MainCharacterRenderer.color = DimColor;
}