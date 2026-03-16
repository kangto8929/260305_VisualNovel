using TMPro;
using UnityEngine;

public class NameInput : MonoBehaviour
{
    public TMP_InputField LastNameInput;
    public TMP_InputField FirstNameInput;
    public string FinalLastName;
    public string FinalFirstName;
    public GameObject ConfirmPopup;
    public TMP_Text PopupNameText;

    public StoryLoader storyLoader; // 인스펙터에서 연결

    void Start()
    {
        ConfirmPopup.SetActive(false);
    }

    public void ConfirmName()
    {
        if (string.IsNullOrWhiteSpace(LastNameInput.text))
            FinalLastName = LastNameInput.placeholder.GetComponent<TMP_Text>().text;
        else
            FinalLastName = LastNameInput.text;

        if (string.IsNullOrWhiteSpace(FirstNameInput.text))
            FinalFirstName = FirstNameInput.placeholder.GetComponent<TMP_Text>().text;
        else
            FinalFirstName = FirstNameInput.text;

        PopupNameText.text = FinalLastName + FinalFirstName + "\n이 이름으로 시작할까요?";
        ConfirmPopup.SetActive(true);
    }

    public void ConfirmFinalName()
    {
        GameManager.Instance.SetPlayerName(FinalLastName, FinalFirstName);

        ConfirmPopup.SetActive(false);

        // 이름 입력 UI 끄고 스토리 시작
        gameObject.SetActive(false);
        storyLoader.LoadChapterAndJump("CH1", "S010");
    }

    public void CancelPopup()
    {
        ConfirmPopup.SetActive(false);
    }

    void Update()
    {
        if (ConfirmPopup.activeSelf && Input.GetKeyDown(KeyCode.Escape))
            CancelPopup();
    }
}