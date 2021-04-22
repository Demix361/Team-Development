using System.Collections.Generic;

/// <summary>
/// Класс данных о камнях.
/// </summary>
[System.Serializable]
public class GemData
{
    /// <summary>
    /// Список состояний красных камней.
    /// </summary>
    public List<bool> redGems;
    /// <summary>
    /// Список состояний зеленых камней камней.
    /// </summary>
    public List<bool> greenGems;
    /// <summary>
    /// Список состояний синих камней.
    /// </summary>
    public List<bool> blueGems;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="gemInfo">Словарь состояний камней</param>
    public GemData(Dictionary<string, List<bool>> gemInfo)
    {
        redGems = gemInfo["RedGem"];
        greenGems = gemInfo["GreenGem"];
        blueGems = gemInfo["BlueGem"];
    }
}
