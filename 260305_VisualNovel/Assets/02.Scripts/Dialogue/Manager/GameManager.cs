using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private Dictionary<string, int> _affection = new Dictionary<string, int>();

    void Awake()
    {
        Instance = this;
    }

    public void AddAffection(string character, int value)
    {
        if (!_affection.ContainsKey(character))
            _affection[character] = 0;

        _affection[character] += value;

        UnityEngine.Debug.Log($"{character} 호감도 변화: {_affection[character]}");
    }

    public int GetAffection(string character)
    {
        if (_affection.ContainsKey(character))
            return _affection[character];
        return 0;
    }
}