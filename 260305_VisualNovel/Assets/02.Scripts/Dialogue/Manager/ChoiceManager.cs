using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class ChoiceManager : MonoBehaviour
{
    public static ChoiceManager Instance;

    [Header("Choice UI")]
    public GameObject ChoiceLayer;       // VerticalLayoutGroup이 붙은 부모
    public GameObject ChoiceButtonPrefab; // 일반 선택지 버튼
    public GameObject ImportantButtonPrefab; // 중요한 선택지 버튼

    private List<GameObject> _activeButtons = new List<GameObject>();

    void Awake()
    {
        Instance = this;
    }

    // 선택지 표시
    public void ShowChoices(List<ChoiceData> choices)
    {
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
            ChoiceData capturedChoice = choice; // 람다에서 안전하게 사용
            button.onClick.AddListener(() => OnChoiceSelected(capturedChoice));

            _activeButtons.Add(buttonObj);
        }

        ChoiceLayer.SetActive(true);
    }

    // 선택지 클릭 시
    private void OnChoiceSelected(ChoiceData choice)
    {
        // 호감도 반영
        if (!string.IsNullOrEmpty(choice.AffectCharacter))
        {
            GameManager.Instance.AddAffection(choice.AffectCharacter, choice.AffectValue);
        }

        // 다음 대사 파일 로드
        StoryManager.Instance.LoadStory(choice.NextFile);

        // 선택지 UI 숨기기
        ClearChoices();
    }

    public void ClearChoices()
    {
        foreach (var btn in _activeButtons)
        {
            Destroy(btn);
        }
        _activeButtons.Clear();
        ChoiceLayer.SetActive(false);
    }
}