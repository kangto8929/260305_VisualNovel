
// =====================================
// StoryLoader
// =====================================
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class StoryLoader : MonoBehaviour
{
    public string StoryFolder = "Assets/Story";

    private Queue<DialogData> _dialogQueue;
    private List<ChoiceData> _currentChoices;

    void Start() => LoadStory("C1_start");

    public void LoadStory(string fileName)
    {
        _dialogQueue = new Queue<DialogData>();
        _currentChoices = new List<ChoiceData>();

        string path = Path.Combine(StoryFolder, fileName + ".txt");
        if (!File.Exists(path))
        {
            Debug.LogError("Story file not found: " + path);
            return;
        }

        string[] lines = File.ReadAllLines(path);

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#") && !line.StartsWith("# Choices"))
                continue;

            if (line.StartsWith("# Choices"))
            {
                _currentChoices = new List<ChoiceData>();
                for (int j = i + 1; j < lines.Length; j++)
                {
                    string choiceLine = lines[j].Trim();
                    if (string.IsNullOrWhiteSpace(choiceLine) || choiceLine.StartsWith("#"))
                        break;

                    string[] cData = choiceLine.Split(',');
                    ChoiceData choice = new ChoiceData
                    {
                        Text = cData[0].Trim(),
                        NextFile = cData[1].Trim(),
                        Important = cData.Length > 2 && cData[2].Trim().ToLower() == "true",
                        AffectCharacter = cData.Length > 3 ? cData[3].Trim() : "",
                        AffectValue = 0
                    };
                    if (cData.Length > 4)
                        int.TryParse(cData[4].Trim(), out choice.AffectValue);

                    _currentChoices.Add(choice);
                }
                continue;
            }

            string[] parts = line.Split('|');
            if (parts.Length < 3) continue;

            DialogType type = (DialogType)System.Enum.Parse(typeof(DialogType), parts[0]);
            DialogData data = new DialogData
            {
                Type = type,
                Speaker = parts[1].Trim(),
                Text = parts[2].Trim(),
                Expression = parts.Length > 3 ? parts[3].Trim() : "",
                Background = parts.Length > 4 ? parts[4].Trim() : "",
                ShowMainCharacter = parts.Length > 5 ? bool.Parse(parts[5].Trim()) : true
            };
            _dialogQueue.Enqueue(data);
        }

        ShowNext();
    }

    public void ShowNext()
    {
        if (_dialogQueue.Count > 0)
        {
            DialogData data = _dialogQueue.Dequeue();
            DialogueManager.Instance.ShowDialogue(data);
        }
        else
            CheckForChoices();
    }

    public void CheckForChoices()
    {
        if (_dialogQueue.Count == 0 && _currentChoices.Count > 0)
        {
            ChoiceManager.Instance.ShowChoices(_currentChoices);
            _currentChoices.Clear();
        }
    }

    public bool HasChoicesAhead() => _currentChoices.Count > 0;
    public bool HasNextDialogue() => _dialogQueue.Count > 0;
}

