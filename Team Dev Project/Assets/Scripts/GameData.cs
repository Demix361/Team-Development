/// <summary>
/// Класс информации о состоянии уровней.
/// </summary>
[System.Serializable]
public class GameData
{
    /// <summary>
    /// Список состояния уровней.
    /// </summary>
    public bool[] unlockedLevels = new bool[10];

    public GameData (bool[] levelInfo)
    {
        unlockedLevels = levelInfo;
    }
}
