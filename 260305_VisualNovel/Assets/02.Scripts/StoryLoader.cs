using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class StoryLoader : MonoBehaviour
{
    public static StoryLoader Instance;

    private string _storyFolder;
    private string _currentChapter;

    private Dictionary<string, NodeData> _nodes = new Dictionary<string, NodeData>();
    private Queue<DialogData> _dialogQueue = new Queue<DialogData>();
    private List<ChoiceData> _currentChoices = new List<ChoiceData>();

    void Awake() => Instance = this;

    void Start()
    {
        _storyFolder = Path.Combine(Application.streamingAssetsPath, "Story");
    }

    // ─────────────────────────────────────
    // {name}, {fname} 치환 (파싱 시점)
    // {char:이름} 은 원본 보존 → 출력 직전 ProcessCharTag() 처리
    // ─────────────────────────────────────
    private string ProcessJosaExceptChar(string text)
    {
        string fullName = GameManager.Instance != null ? GameManager.Instance.PlayerName : "주인공";
        string firstName = GameManager.Instance != null ? GameManager.Instance.PlayerFirstName : "주인공";

        text = text.Replace("{name}", fullName);
        text = text.Replace("{name:아}", fullName + KoreanJosa.Ah(fullName));
        text = text.Replace("{name:이}", fullName + KoreanJosa.Ee(fullName));
        text = text.Replace("{name:이가}", fullName + KoreanJosa.Ga(fullName));
        text = text.Replace("{name:은}", fullName + KoreanJosa.Neun(fullName));
        text = text.Replace("{name:을}", fullName + KoreanJosa.Eul(fullName));
        text = text.Replace("{name:과}", fullName + KoreanJosa.Gwa(fullName));
        text = text.Replace("{name:으로}", fullName + KoreanJosa.Ro(fullName));
        text = text.Replace("{name:이랑}", fullName + KoreanJosa.Rang(fullName));
        text = text.Replace("{name:이 형}", fullName + KoreanJosa.Hyung(fullName));
        text = text.Replace("{name:이 누나}", fullName + KoreanJosa.Nuna(fullName));
        text = text.Replace("{name:이 언니}", fullName + KoreanJosa.Unni(fullName));
        text = text.Replace("{name:이 오빠}", fullName + KoreanJosa.Oppa(fullName));
        text = text.Replace("{name:이/가}", fullName + KoreanJosa.Iga(fullName));

        text = text.Replace("{fname}", firstName);
        text = text.Replace("{fname:아}", firstName + KoreanJosa.Ah(firstName));
        text = text.Replace("{fname:이}", firstName + KoreanJosa.Ee(firstName));
        text = text.Replace("{fname:이가}", firstName + KoreanJosa.Ga(firstName));
        text = text.Replace("{fname:은}", firstName + KoreanJosa.Neun(firstName));
        text = text.Replace("{fname:을}", firstName + KoreanJosa.Eul(firstName));
        text = text.Replace("{fname:과}", firstName + KoreanJosa.Gwa(firstName));
        text = text.Replace("{fname:으로}", firstName + KoreanJosa.Ro(firstName));
        text = text.Replace("{fname:이랑}", firstName + KoreanJosa.Rang(firstName));
        text = text.Replace("{fname:이 형}", firstName + KoreanJosa.Hyung(firstName));
        text = text.Replace("{fname:이 누나}", firstName + KoreanJosa.Nuna(firstName));
        text = text.Replace("{fname:이 언니}", firstName + KoreanJosa.Unni(firstName));
        text = text.Replace("{fname:이 오빠}", firstName + KoreanJosa.Oppa(firstName));
        text = text.Replace("{fname:이/가}", firstName + KoreanJosa.Iga(firstName));

        return text;
    }

    // ─────────────────────────────────────
    // {char:캐릭터명} 치환 (출력 직전)
    // GameManager.GetDisplayName() 으로 통합 처리
    // ─────────────────────────────────────
    private string ProcessCharTag(string text)
    {
        return System.Text.RegularExpressions.Regex.Replace(
            text,
            @"\{char:([^}]+)\}",
            match =>
            {
                string charName = match.Groups[1].Value;
                return GameManager.Instance != null
                    ? GameManager.Instance.GetDisplayName(charName)
                    : charName;
            }
        );
    }

    // ─────────────────────────────────────
    // 챕터 파일 파싱
    // ─────────────────────────────────────
    public void LoadChapter(string chapterName)
    {
        _nodes.Clear();
        _currentChapter = chapterName;

        string path = Path.Combine(_storyFolder, chapterName + ".txt");
        if (!File.Exists(path))
        {
            Debug.LogError("파일 없음: " + path);
            return;
        }

        string[] lines = File.ReadAllLines(path);

        string currentNodeId = null;
        List<DialogData> currentDialogs = null;
        List<ChoiceData> currentChoices = null;
        bool inChoice = false;

        foreach (string rawLine in lines)
        {
            string line = rawLine.Trim();
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#")) continue;

            if (line.StartsWith("::"))
            {
                if (currentNodeId != null)
                    _nodes[currentNodeId] = new NodeData(currentDialogs, currentChoices);

                currentNodeId = line.Substring(2).Trim();
                currentDialogs = new List<DialogData>();
                currentChoices = new List<ChoiceData>();
                inChoice = false;
                continue;
            }

            if (currentNodeId == null) continue;

            if (line == "*choice") { inChoice = true; continue; }

            // *hide: 이름 숨김
            if (line.StartsWith("*hide "))
            {
                string character = line.Substring("*hide ".Length).Trim();
                currentDialogs.Add(new DialogData
                {
                    Type = DialogType.Narration,
                    Speaker = "__hide__",
                    Text = character
                });
                continue;
            }

            // *reveal: 이름 공개
            if (line.StartsWith("*reveal "))
            {
                string character = line.Substring("*reveal ".Length).Trim();
                currentDialogs.Add(new DialogData
                {
                    Type = DialogType.Narration,
                    Speaker = "__reveal__",
                    Text = character
                });
                continue;
            }

            if (inChoice)
            {
                string[] c = line.Split(',');
                if (c.Length < 2) continue;

                ChoiceData choice = new ChoiceData
                {
                    Text = ProcessJosaExceptChar(c[0].Trim()),
                    NextNode = c[1].Trim(),
                    Important = c.Length > 2 && c[2].Trim().ToLower() == "true",
                    AffectCharacter = c.Length > 3 ? c[3].Trim() : "",
                    AffectValue = 0
                };
                if (c.Length > 4) int.TryParse(c[4].Trim(), out choice.AffectValue);
                currentChoices.Add(choice);
            }
            else
            {
                string[] parts = line.Split('|');
                if (parts.Length < 1) continue;

                string type = parts[0].Trim();

                if (type == "NameChange")
                {
                    // NameChange는 현재 미사용이지만 구조 유지
                    continue;
                }

                if (parts.Length < 3) continue;

                DialogData data = new DialogData
                {
                    Type = (DialogType)System.Enum.Parse(typeof(DialogType), type),
                    Speaker = parts[1].Trim(),
                    Text = ProcessJosaExceptChar(parts[2].Trim()),
                    Expression = parts.Length > 3 ? parts[3].Trim() : "",
                    Background = parts.Length > 4 ? parts[4].Trim() : "",
                    ShowMainCharacter = parts.Length > 5 ? bool.Parse(parts[5].Trim()) : true,
                    MainExpression = parts.Length > 6 ? parts[6].Trim() : ""
                };
                currentDialogs.Add(data);
            }
        }

        if (currentNodeId != null)
            _nodes[currentNodeId] = new NodeData(currentDialogs, currentChoices);

        Debug.Log($"[StoryLoader] {chapterName} 로드 완료 - 노드 수: {_nodes.Count}");
    }

    public void LoadChapterAndJump(string chapterName, string nodeId)
    {
        LoadChapter(chapterName);
        JumpToNode(nodeId);
    }

    public void JumpToNode(string nodeId)
    {
        if (!_nodes.ContainsKey(nodeId))
        {
            Debug.LogError($"[StoryLoader] 노드 없음: {nodeId} (챕터: {_currentChapter})");
            return;
        }

        NodeData node = _nodes[nodeId];
        _dialogQueue = new Queue<DialogData>(node.Dialogs);
        _currentChoices = new List<ChoiceData>(node.Choices);

        ShowNext();
    }

    public void OnChoiceSelected(ChoiceData choice)
    {
        string next = choice.NextNode;
        if (next.Contains("::"))
        {
            string[] split = next.Split(new string[] { "::" }, System.StringSplitOptions.None);
            LoadChapterAndJump(split[0].Trim(), split[1].Trim());
        }
        else
            JumpToNode(next);
    }

    public void ShowNext()
    {
        if (_dialogQueue.Count == 0) { CheckForChoices(); return; }

        DialogData next = _dialogQueue.Dequeue();

        // 이름 숨김 커맨드
        if (next.Speaker == "__hide__")
        {
            GameManager.Instance.HideCharacter(next.Text);
            ShowNext();
            return;
        }

        // 이름 공개 커맨드
        if (next.Speaker == "__reveal__")
        {
            GameManager.Instance.RevealCharacter(next.Text);
            ShowNext();
            return;
        }

        // {char:이름} 태그 치환 (출력 직전)
        next.Text = ProcessCharTag(next.Text);

        DialogueManager.Instance.ShowDialogue(next);
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

public class NodeData
{
    public List<DialogData> Dialogs;
    public List<ChoiceData> Choices;

    public NodeData(List<DialogData> dialogs, List<ChoiceData> choices)
    {
        Dialogs = dialogs;
        Choices = choices;
    }
}