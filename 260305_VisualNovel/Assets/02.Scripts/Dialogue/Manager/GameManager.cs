using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public string PlayerName = "";
    public string PlayerLastName = "";
    public string PlayerFirstName = "";

    private Dictionary<string, int> _affection = new Dictionary<string, int>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    public void SetPlayerName(string lastName, string firstName)
    {
        PlayerFirstName = firstName;
        PlayerLastName = lastName; 
        PlayerName = lastName + firstName; 
    }

    public void AddAffection(string character, int value)
    {
        if (!_affection.ContainsKey(character))
            _affection[character] = 0;

        int before = _affection[character];
        _affection[character] += value;
        int after = _affection[character];

        string sign = value >= 0 ? "+" : "";
        Debug.Log($"[È£°¨µµ] {character} | {before} ¡æ {after} ({sign}{value})");
    }

    public int GetAffection(string character)
    {
        if (_affection.ContainsKey(character))
            return _affection[character];
        return 0;
    }
}