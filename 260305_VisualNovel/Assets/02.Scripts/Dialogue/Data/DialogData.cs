[System.Serializable]

public enum DialogType
{
    Narration,
    Main,
    Sub
}

public class DialogData
{
    public DialogType Type;
    public string Speaker;
    public string Text;
    public string Expression;
    public string Background;
    public bool ShowMainCharacter = false;
    public string MainExpression;   //메인 캐릭터 표정 (Sub/Narration일 때 사용)
}