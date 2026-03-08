using UnityEngine;
using TMPro;

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

    void Awake()
    {
        Instance = this;
    }

    public void ShowDialogue(DialogData data)
    {
        HideAll();

        // 배경 변경
        if (!string.IsNullOrEmpty(data.Background) && BackgroundManager != null)
        {
            BackgroundManager.ChangeBackground(data.Background);
        }

        // 메인 캐릭터 표시 여부 처리
        if (MainCharacterManager != null)
        {
            // true이면 Renderer 활성, false이면 비활성
            MainCharacterManager.MainCharacterRenderer.gameObject.SetActive(data.ShowMainCharacter);
            if (data.ShowMainCharacter)
            {
                MainCharacterManager.SetCharacterSprite(data.Speaker, data.Expression);
            }
        }

        switch (data.Type)
        {
            case DialogType.Narration:
                Narration.SetActive(true);
                NarrationText.text = data.Text;
                break;

            case DialogType.Main:
                Dialogue.SetActive(true);
                NameText.text = data.Speaker;
                DialogueText.text = data.Text;
                break;

            case DialogType.Sub:
                SubDialogue.SetActive(true);
                SubNameText.text = data.Speaker;
                SubDialogueText.text = data.Text;

                if (SubCharacterManager != null)
                    SubCharacterManager.ShowSubCharacter(data.Speaker);
                break;
        }
    }

    void HideAll()
    {
        Narration.SetActive(false);
        Dialogue.SetActive(false);
        SubDialogue.SetActive(false);
    }
}