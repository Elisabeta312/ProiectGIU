public static class GameSessionState
{
    public static bool IsNewGameStart { get; private set; } = false;
    public static bool IsLoadingSave { get; private set; } = false;

    public static void MarkNewGame()
    {
        IsNewGameStart = true;
        IsLoadingSave = false;
    }

    public static void MarkLoadGame()
    {
        IsNewGameStart = false;
        IsLoadingSave = true;
    }

    public static void Clear()
    {
        IsNewGameStart = false;
        IsLoadingSave = false;
    }
}