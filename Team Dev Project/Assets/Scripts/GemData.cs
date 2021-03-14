using System.Collections.Generic;

[System.Serializable]
public class GemData
{
    public List<bool> redGems;
    public List<bool> greenGems;
    public List<bool> blueGems;

    public GemData(Dictionary<string, List<bool>> gemInfo)
    {
        redGems = gemInfo["RedGem"];
        greenGems = gemInfo["GreenGem"];
        blueGems = gemInfo["BlueGem"];
    }
}
