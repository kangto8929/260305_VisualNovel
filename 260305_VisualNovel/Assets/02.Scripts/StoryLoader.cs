using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class StoryLoader : MonoBehaviour
{
    public string StoryFolder = "Assets/Story";

    private Queue<DialogData> _dialogQueue;
    private List<ChoiceData> _currentChoices;

    void Start()
    {
        LoadStory("C1_start");
    }

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
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#") && !line.StartsWith("# Choices")) continue;

            // 선택지 블록 처리
            if (line.StartsWith("# Choices"))
            {
                _currentChoices = new List<ChoiceData>();

                // 선택지 여러 줄 읽기
                for (int j = i + 1; j < lines.Length; j++)
                {
                    string choiceLine = lines[j].Trim();
                    if (string.IsNullOrWhiteSpace(choiceLine) || choiceLine.StartsWith("#")) break;

                    string[] cData = choiceLine.Split(',');

                    ChoiceData choice = new ChoiceData
                    {
                        Text = cData[0].Trim(),
                        NextFile = cData[1].Trim(),
                        Important = cData.Length > 2 && cData[2].Trim().ToLower() == "true",
                        AffectCharacter = cData.Length > 3 ? cData[3].Trim() : "",
                        AffectValue = 0
                    };

                    // AffectValue 안전하게 처리
                    if (cData.Length > 4)
                    {
                        int.TryParse(cData[4].Trim(), out int result);
                        choice.AffectValue = result;
                    }

                    _currentChoices.Add(choice);
                }

                continue; // 선택지 처리 후 다음 줄로
            }

            // 대사 파싱
            string[] parts = line.Split('|');
            if (parts.Length < 3) continue;

            DialogType type = (DialogType)System.Enum.Parse(typeof(DialogType), parts[0]);
            string speaker = parts[1].Trim();
            string text = parts[2].Trim();
            string expression = parts.Length > 3 ? parts[3].Trim() : "";
            string background = parts.Length > 4 ? parts[4].Trim() : "";
            bool showMain = parts.Length > 5 ? bool.Parse(parts[5].Trim()) : true;

            DialogData data = new DialogData
            {
                Type = type,
                Speaker = speaker,
                Text = text,
                Expression = expression,
                Background = background,
                ShowMainCharacter = showMain
            };

            _dialogQueue.Enqueue(data);
        }

        // 첫 대사 또는 선택지 표시
        ShowNext();
    }

    public void ShowNext()
    {
        if (_dialogQueue.Count > 0)
        {
            DialogData data = _dialogQueue.Dequeue();
            DialogueManager.Instance.ShowDialogue(data);
        }
        else if (_currentChoices.Count > 0)
        {
            ChoiceManager.Instance.ShowChoices(_currentChoices);
            _currentChoices.Clear(); // 선택지 표시 후 초기화
        }
        else
        {
            Debug.Log("Story Ended");
        }
    }
}