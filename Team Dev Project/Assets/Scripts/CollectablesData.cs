/// <summary>
/// Класс данных собираемых предметов.
/// </summary>
/// <remarks>
/// Хранит информацию о собираемых предметах.
/// </remarks>
[System.Serializable]
public class CollectablesData
{
    /// <summary>
    /// Количество красных камней.
    /// </summary>
    public int redGemNum;
    /// <summary>
    /// Количество зеленых камней.
    /// </summary>
    public int greenGemNum;
    /// <summary>
    /// Количество синих камней.
    /// </summary>
    public int blueGemNum;
    /// <summary>
    /// Количество золотых монет.
    /// </summary>
    public int goldNum;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="red">Количество красных камней.</param>
    /// <param name="green">Количество зеленых камней.</param>
    /// <param name="blue">Количество синих камней.</param>
    /// <param name="gold">Количество золотых монет.</param>
    public CollectablesData(int red, int green, int blue, int gold)
    {
        redGemNum = red;
        greenGemNum = green;
        blueGemNum = blue;
        goldNum = gold;
    }
}
