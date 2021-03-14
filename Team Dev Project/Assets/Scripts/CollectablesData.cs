[System.Serializable]
public class CollectablesData
{
    public int redGemNum;
    public int greenGemNum;
    public int blueGemNum;
    public int goldNum;

    public CollectablesData(int red, int green, int blue, int gold)
    {
        redGemNum = red;
        greenGemNum = green;
        blueGemNum = blue;
        goldNum = gold;
    }
}
