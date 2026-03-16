using System;

[Serializable]
public class ChoiceData
{
    public string Text;        // 버튼에 표시될 텍스트
    public string NextNode;    // 다음에 이동할 노드 ID 저장
    public bool Important;     // 중요한 선택지 여부
    public string AffectCharacter; // 호감도 변화를 줄 캐릭터 이름
    public int AffectValue;        // 호감도 증감 값
}