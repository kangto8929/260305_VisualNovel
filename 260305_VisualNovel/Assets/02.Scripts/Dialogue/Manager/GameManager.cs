using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public string PlayerName = "";
    public string PlayerLastName = "";
    public string PlayerFirstName = "";

    private Dictionary<string, int> _affection = new Dictionary<string, int>();

    private Dictionary<string, string> _hiddenCharacters = new Dictionary<string, string>();
    private HashSet<string> _revealedCharacters = new HashSet<string>();

    // character: 실제 캐릭터 키값 (예: "이상")
    // customName: reveal 전 표시될 이름 (예: "라임머리색 남자")
    public void HideCharacter(string character, string customName)
    {
        _hiddenCharacters[character] = customName;
        Debug.Log($"[이름 숨김] {character} → {customName}");
    }

    public void RevealCharacter(string character)
    {
        _revealedCharacters.Add(character);
        Debug.Log($"[이름 공개] {character}");
    }

    // 모든 캐릭터 이름 표시 여부를 여기서 통합 처리
    public string GetDisplayName(string character)
    {
        if (_hiddenCharacters.ContainsKey(character) && !_revealedCharacters.Contains(character))
            return _hiddenCharacters[character]; // "라임머리색 남자" 등
        return character;
    }

    public bool IsCharacterRevealed(string character)
    {
        return _revealedCharacters.Contains(character);
    }

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
        Debug.Log($"[호감도] {character} | {before} → {after} ({sign}{value})");
    }

    public int GetAffection(string character)
    {
        if (_affection.ContainsKey(character))
            return _affection[character];
        return 0;
    }
}