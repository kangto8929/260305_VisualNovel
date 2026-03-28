using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

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

    [Header("Auto 버튼 이미지")]
    public Image AutoButtonImage;

    Coroutine typingCoroutine;
    Coroutine autoCoroutine;

    bool _isTyping;
    string _currentText;
    TextMeshProUGUI _currentTarget;

    bool _autoMode = false;
    bool _skipMode = false;

    string _currentMainCharacter = "";

    void Awake() => Instance = this;

    // ─────────────────────────────────────
    // 이름 표시: GameManager 하나로 통합
    // Main이든 Sub이든 동일하게 처리
    // ─────────────────────────────────────
    private string GetDisplayName(string speaker)
    {
        if (GameManager.Instance != null)
            return GameManager.Instance.GetDisplayName(speaker);
        return speaker;
    }

    // ─────────────────────────────────────
    // 대사 표시
    // ─────────────────────────────────────
    public void ShowDialogue(DialogData data)
    {
        Narration.SetActive(false);
        Dialogue.SetActive(false);
        SubDialogue.SetActive(false);

        if (!string.IsNullOrEmpty(data.Background) && BackgroundManager != null)
            BackgroundManager.ChangeBackground(data.Background);

        if (MainCharacterManager != null)
        {
            MainCharacterManager.MainCharacterRenderer.gameObject.SetActive(data.ShowMainCharacter);

            if (data.ShowMainCharacter)
            {
                if (data.Type == DialogType.Main)
                {
                    _currentMainCharacter = data.Speaker;
                    MainCharacterManager.SetCharacterSprite(data.Speaker, data.Expression);
                    MainCharacterManager.SetActive();
                }
                else
                {
                    if (!string.IsNullOrEmpty(data.MainExpression) && !string.IsNullOrEmpty(_currentMainCharacter))
                        MainCharacterManager.SetCharacterSprite(_currentMainCharacter, data.MainExpression);
                    MainCharacterManager.SetDim();
                }
            }
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
                NameText.text = GetDisplayName(data.Speaker);
                _currentTarget = DialogueText;
                break;

            case DialogType.Sub:
                SubDialogue.SetActive(true);
                SubNameText.text = GetDisplayName(data.Speaker);
                _currentTarget = SubDialogueText;
                if (SubCharacterManager != null)
                    SubCharacterManager.ShowSubCharacter(data.Speaker);
                break;
        }

        _currentText = data.Text;
        typingCoroutine = StartCoroutine(TypeText());
    }

    // ─────────────────────────────────────
    // 타이핑
    // ─────────────────────────────────────
    IEnumerator TypeText()
    {
        _isTyping = true;
        _currentTarget.text = "";

        int i = 0;
        while (i < _currentText.Length)
        {
            if (_currentText[i] == '<')
            {
                int closeIndex = _currentText.IndexOf('>', i);
                if (closeIndex != -1)
                {
                    _currentTarget.text += _currentText.Substring(i, closeIndex - i + 1);
                    i = closeIndex + 1;
                    continue;
                }
            }
            _currentTarget.text += _currentText[i];
            i++;
            yield return new WaitForSeconds(TypingSpeed);
        }

        _isTyping = false;
        storyLoader.CheckForChoices();
    }

    // ─────────────────────────────────────
    // 다음 버튼
    // ─────────────────────────────────────
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

    // ─────────────────────────────────────
    // Auto
    // ─────────────────────────────────────
    public void ToggleAuto()
    {
        _autoMode = !_autoMode;
        _skipMode = false;

        if (AutoButtonImage != null)
            AutoButtonImage.color = _autoMode ? Color.black : Color.white;

        if (_autoMode)
        {
            if (autoCoroutine != null) StopCoroutine(autoCoroutine);
            autoCoroutine = StartCoroutine(AutoPlay());
        }
        else
        {
            if (autoCoroutine != null) { StopCoroutine(autoCoroutine); autoCoroutine = null; }
        }
    }

    IEnumerator AutoPlay()
    {
        while (_autoMode)
        {
            yield return new WaitUntil(() => !_isTyping);

            if (ChoiceManager.Instance != null && ChoiceManager.Instance.IsChoiceVisible)
            {
                yield return new WaitUntil(() =>
                    ChoiceManager.Instance == null || !ChoiceManager.Instance.IsChoiceVisible);
                continue;
            }

            if (!_autoMode) yield break;
            yield return new WaitForSeconds(AutoDelay);
            if (!_autoMode) yield break;
            if (_isTyping) continue;

            OnClickNext();
        }
    }

    public void ResumeAutoIfActive()
    {
        if (_autoMode)
        {
            if (autoCoroutine != null) StopCoroutine(autoCoroutine);
            autoCoroutine = StartCoroutine(AutoPlay());
        }
    }

    // ─────────────────────────────────────
    // Skip
    // ─────────────────────────────────────
    public void ToggleSkip()
    {
        _skipMode = !_skipMode;
        _autoMode = false;

        if (AutoButtonImage != null) AutoButtonImage.color = Color.white;
        if (autoCoroutine != null) { StopCoroutine(autoCoroutine); autoCoroutine = null; }

        if (_skipMode) StartCoroutine(SkipToChoice());
    }

    IEnumerator SkipToChoice()
    {
        while (_skipMode)
        {
            if (_isTyping && typingCoroutine != null && _currentTarget != null)
            {
                StopCoroutine(typingCoroutine);
                _currentTarget.text = _currentText;
                _isTyping = false;
            }

            if (storyLoader.HasNextDialogue())
                storyLoader.ShowNext();
            else
            {
                _skipMode = false;
                storyLoader.CheckForChoices();
                yield break;
            }

            yield return null;
        }
    }

    public void StopSkip() { _skipMode = false; }
}