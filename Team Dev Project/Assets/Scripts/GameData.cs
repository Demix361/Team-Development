
public class GameData
{
    public bool[] unlockedLevels = new bool[10];

    public GameData (bool[] levelInfo)
    {
        unlockedLevels = levelInfo;
    }
}
