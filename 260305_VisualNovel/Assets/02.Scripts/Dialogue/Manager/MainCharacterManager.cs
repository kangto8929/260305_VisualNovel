using UnityEngine;
using System.Collections.Generic;

public class MainCharacterManager : MonoBehaviour
{
    public SpriteRenderer MainCharacterRenderer;
    public List<MainCharacter> Characters;

    private Dictionary<string, Dictionary<string, Sprite>> _characterDict;

    void Awake()
    {
        _characterDict = new Dictionary<string, Dictionary<string, Sprite>>();

        foreach (var character in Characters)
        {
            var expressionDict = new Dictionary<string, Sprite>();
            foreach (var exp in character.Expressions)
            {
                expressionDict[exp.Expression] = exp.Portrait;
            }
            _characterDict[character.CharacterName] = expressionDict;
        }
    }

    // 단순히 스프라이트만 바꾸는 역할
    public void SetCharacterSprite(string characterName, string expression)
    {
        if (string.IsNullOrEmpty(characterName) || string.IsNullOrEmpty(expression)) return;

        if (!_characterDict.TryGetValue(characterName, out var expDict)) return;
        if (!expDict.TryGetValue(expression, out var sprite)) return;

        MainCharacterRenderer.sprite = sprite;
    }
}