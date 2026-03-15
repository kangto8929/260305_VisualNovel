using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;

// =====================================
// DialogueManager
// =====================================
public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("나레이션 대사창")]
    public GameObject Narration;
    public TextMeshProUGUI NarrationText;

    [Header("주연 대사창")]
    public GameObject Dialogue;
    public TextMeshProUGUI NameText;
    public TextMeshProUGUI DialogueText;

    [Header("조연 대사창")]
    public GameObject SubDialogue;
    public TextMeshProUGUI SubNameText;
    public TextMeshProUGUI SubDialogueText;

    [Header("캐릭터 매니저")]
    public MainCharacterManager MainCharacterManager;
    public SubCharacterManager SubCharacterManager;

    [Header("배경 매니저")]
    public BackgroundManager BackgroundManager;

    [Header("스토리 로더")]
    public StoryLoader storyLoader;

    [Header("타이핑 설정")]
    public float TypingSpeed = 0.03f;

    [Header("Auto/Skip 설정")]
    public float AutoDelay = 1.5f;

    Coroutine typingCoroutine;

    bool _isTyping;
    string _currentText;
    TextMeshProUGUI _currentTarget;

    bool _autoMode = false;
    bool _skipMode = false;

    void Awake() => Instance = this;

    public void ShowDialogue(DialogData data)
    {
        HideAll();

        if (!string.IsNullOrEmpty(data.Background) && BackgroundManager != null)
            BackgroundManager.ChangeBackground(data.Background);

        if (MainCharacterManager != null)
        {
            MainCharacterManager.MainCharacterRenderer.gameObject.SetActive(data.ShowMainCharacter);
            if (data.ShowMainCharacter)
                MainCharacterManager.SetCharacterSprite(data.Speaker, data.Expression);
        }

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        switch (data.Type)
        {
            case DialogType.Narration:
                Narration.SetActive(true);
                _currentTarget = NarrationText;
                break;
            case DialogType.Main:
                Dialogue.SetActive(true);
                NameText.text = data.Speaker;
                _currentTarget = DialogueText;
                break;
            case DialogType.Sub:
                SubDialogue.SetActive(true);
                SubNameText.text = data.Speaker;
                _currentTarget = SubDialogueText;
                if (SubCharacterManager != null)
                    SubCharacterManager.ShowSubCharacter(data.Speaker);
                break;
        }

        _currentText = data.Text;
        typingCoroutine = StartCoroutine(TypeText());
    }

    IEnumerator TypeText()
    {
        _isTyping = true;
        _currentTarget.text = "";

        foreach (char c in _currentText)
        {
            _currentTarget.text += c;
            yield return new WaitForSeconds(TypingSpeed);
        }

        _isTyping = false;
        storyLoader.CheckForChoices();
    }

    public void OnClickNext()
    {
        if (_isTyping)
        {
            StopCoroutine(typingCoroutine);
            _currentTarget.text = _currentText;
            _isTyping = false;
            storyLoader.CheckForChoices();
            return;
        }
        storyLoader.ShowNext();
    }

    public void ToggleAuto()
    {
        _autoMode = !_autoMode;
        if (_autoMode)
        {
            _skipMode = false;
            StartCoroutine(AutoPlay());
        }
    }

    IEnumerator AutoPlay()
    {
        while (_autoMode)
        {
            if (!_isTyping)
            {
                yield return new WaitForSeconds(AutoDelay);
                OnClickNext();
            }
            yield return null;
        }
    }

    public void ToggleSkip()
    {
        _skipMode = !_skipMode;
        _autoMode = false;

        if (_skipMode)
        {
            // 현재 타이핑 중이면 즉시 끝내기
            if (_isTyping && typingCoroutine != null && _currentTarget != null)
            {
                StopCoroutine(typingCoroutine);
                _currentTarget.text = _currentText;
                _isTyping = false;
            }

            StartCoroutine(SkipToChoice());
        }
    }

    IEnumerator SkipToChoice()
    {
        while (_skipMode)
        {
            // 타이핑 중이면 즉시 끝내기
            if (_isTyping && typingCoroutine != null && _currentTarget != null)
            {
                StopCoroutine(typingCoroutine);
                _currentTarget.text = _currentText;
                _isTyping = false;
            }

            //  순서 변경: 남은 대사가 있으면 먼저 소진
            if (storyLoader.HasNextDialogue())
            {
                storyLoader.ShowNext();
            }
            else
            {
                // 대사가 없을 때만 선택지 표시 후 종료
                _skipMode = false;
                storyLoader.CheckForChoices();
                yield break;
            }

            yield return null;
        }
    }

    public void StopAutoSkip()
    {
        _autoMode = false;
        _skipMode = false;
    }

    void HideAll()
    {
        Narration.SetActive(false);
        Dialogue.SetActive(false);
        SubDialogue.SetActive(false);
    }
}
