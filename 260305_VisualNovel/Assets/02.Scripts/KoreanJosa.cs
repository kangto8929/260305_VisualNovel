public static class KoreanJosa
{
    private static readonly char[] EnglishVowels = { 'a', 'e', 'i', 'o', 'u', 'A', 'E', 'I', 'O', 'U' };

    public static bool HasBatchim(string word)
    {
        if (string.IsNullOrEmpty(word)) return false;

        char lastChar = word[word.Length - 1];

        // 한글
        if (lastChar >= 0xAC00 && lastChar <= 0xD7A3)
            return (lastChar - 0xAC00) % 28 != 0;

        // 영어
        if ((lastChar >= 'a' && lastChar <= 'z') || (lastChar >= 'A' && lastChar <= 'Z'))
            return System.Array.IndexOf(EnglishVowels, lastChar) == -1;

        // 숫자
        if (char.IsDigit(lastChar))
        {
            switch (lastChar)
            {
                case '1': case '3': case '6': case '7': case '8': return true;
                default: return false;
            }
        }

        return false;
    }

    public static string Pick(string word, string withBatchim, string withoutBatchim)
        => HasBatchim(word) ? withBatchim : withoutBatchim;

    public static string Ah(string word) => Pick(word, "아", "야");
    public static string Ee(string word) => Pick(word, "이", "");
    public static string Ga(string word) => Pick(word, "이가", "가");

    // 서준혁이 / 서민지가
    public static string Iga(string word) => Pick(word, "이", "가");

    public static string Neun(string word) => Pick(word, "은", "는");
    public static string Eul(string word) => Pick(word, "을", "를");
    public static string Gwa(string word) => Pick(word, "과", "와");
    public static string Ro(string word) => Pick(word, "으로", "로");
    public static string Rang(string word) => Pick(word, "이랑", "랑");
    public static string Hyung(string word) => Pick(word, "이 형", " 형");
    public static string Nuna(string word) => Pick(word, "이 누나", " 누나");
    public static string Unni(string word) => Pick(word, "이 언니", " 언니");
    public static string Oppa(string word) => Pick(word, "이 오빠", " 오빠");
}