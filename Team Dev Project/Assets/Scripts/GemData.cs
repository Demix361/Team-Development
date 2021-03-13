using System.Collections.Generic;

[System.Serializable]
public class GemData
{
    public bool[] redGems;
    public bool[] greenGems;
    public bool[] blueGems;

    public GemData(Dictionary<string, bool[]> gemInfo)
    {
        redGems = gemInfo["RedGem"];
        greenGems = gemInfo["GreenGem"];
        blueGems = gemInfo["BlueGem"];
    }
}
