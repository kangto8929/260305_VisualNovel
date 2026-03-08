using UnityEngine;
using System.Collections.Generic;

public class BackgroundManager : MonoBehaviour
{
    public SpriteRenderer BackgroundRenderer;

    public List<BackgroundData> Backgrounds;

    private Dictionary<string, Sprite> _backgroundDict;

    void Awake()
    {
        _backgroundDict = new Dictionary<string, Sprite>();

        foreach (var bg in Backgrounds)
        {
            _backgroundDict[bg.Name] = bg.Image;
        }
    }

    public void ChangeBackground(string backgroundName)
    {
        if (_backgroundDict.TryGetValue(backgroundName, out var sprite))
        {
            BackgroundRenderer.sprite = sprite;
        }
    }
}