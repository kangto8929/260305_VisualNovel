// =====================================
// ChoiceManager
// =====================================
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

    void Awake() => Instance = this;

    public void ShowChoices(List<ChoiceData> choices)
    {
        DialogueManager.Instance.StopAutoSkip();
        ClearChoices();

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

        ChoiceLayer.SetActive(true);
    }

    private void OnChoiceSelected(ChoiceData choice)
    {
        if (!string.IsNullOrEmpty(choice.AffectCharacter))
            GameManager.Instance.AddAffection(choice.AffectCharacter, choice.AffectValue);

        StoryLoader loader = FindObjectOfType<StoryLoader>();
        loader.LoadStory(choice.NextFile);

        ClearChoices();
    }

    public void ClearChoices()
    {
        foreach (var btn in _activeButtons)
            Destroy(btn);

        _activeButtons.Clear();
        ChoiceLayer.SetActive(false);
    }
}
