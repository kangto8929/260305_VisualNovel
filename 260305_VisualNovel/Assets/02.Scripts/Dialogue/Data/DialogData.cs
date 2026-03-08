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
}