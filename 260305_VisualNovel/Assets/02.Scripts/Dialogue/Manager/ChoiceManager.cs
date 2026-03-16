using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceManager : MonoBehaviour
{
    public static ChoiceManager Instance;

    [Header("Choice UI")]
    public GameObject ChoiceLayer;
    public GameObject ChoiceButtonPrefab;
    public GameObject ImportantButtonPrefab;

    private List<GameObject> _activeButtons = new List<GameObject>();
    private bool _isShowingChoices = false;

    public bool IsChoiceVisible => _isShowingChoices;

    void Awake() => Instance = this;

    public void ShowChoices(List<ChoiceData> choices)
    {
        // Skip만 중단, Auto는 유지
        DialogueManager.Instance.StopSkip();

        ClearChoices();
        _isShowingChoices = true;

        foreach (var choice in choices)
        {
            GameObject buttonObj = Instantiate(
                choice.Important ? ImportantButtonPrefab : ChoiceButtonPrefab,
                ChoiceLayer.transform
            );

            var buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
                buttonText.text = choice.Text;

            var button = buttonObj.GetComponent<Button>();
            ChoiceData capturedChoice = choice;
            button.onClick.AddListener(() => OnChoiceSelected(capturedChoice));
            _activeButtons.Add(buttonObj);
        }
    }

    private void OnChoiceSelected(ChoiceData choice)
    {
        if (!string.IsNullOrEmpty(choice.AffectCharacter))
            GameManager.Instance.AddAffection(choice.AffectCharacter, choice.AffectValue);

        ClearChoices();
        StoryLoader.Instance.OnChoiceSelected(choice);

        // Auto 상태였으면 재개
        DialogueManager.Instance.ResumeAutoIfActive();
    }

    public void ClearChoices()
    {
        foreach (var btn in _activeButtons)
            Destroy(btn);
        _activeButtons.Clear();
        _isShowingChoices = false;
    }
}