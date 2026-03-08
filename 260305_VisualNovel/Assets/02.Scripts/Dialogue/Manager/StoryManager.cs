using UnityEngine;

public class StoryManager : MonoBehaviour
{
    public static StoryManager Instance;

    void Awake()
    {
        Instance = this;
    }

    public void LoadStory(string storyFileName)
    {
        Debug.Log("다음 스토리 파일: " + storyFileName);

        // 여기서 실제 파일 읽어서 DialogueManager 통해 대사 표시
        // DialogueManager.Instance.ShowDialogue(...)
        // 그리고 선택지가 나오면 ChoiceManager.Instance.ShowChoices(...) 호출
    }
}