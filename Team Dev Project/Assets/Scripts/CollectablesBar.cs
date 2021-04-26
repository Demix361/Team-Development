using UnityEngine;
using TMPro;

/// <summary>
/// Класс панели подбираемых предметов.
/// </summary>
public class CollectablesBar : MonoBehaviour
{
    /// <summary>
    /// Текст красных камней.
    /// </summary>
    [SerializeField]
    private TMP_Text _redGemText;
    /// <summary>
    /// Текст зеленых камней.
    /// </summary>
    [SerializeField]
    private TMP_Text _greenGemText;
    /// <summary>
    /// Текст синих камней.
    /// </summary>
    [SerializeField]
    private TMP_Text _blueGemText;
    /// <summary>
    /// Текст золотых монет.
    /// </summary>
    [SerializeField]
    private TMP_Text _goldText;

    /// <summary>
    /// Установить текст красных камней.
    /// </summary>
    /// <param name="num">Количество камней.</param>
    public void SetRedGemText(int num)
    {
        _redGemText.SetText(num.ToString());
    }

    /// <summary>
    /// Установить текст зеленых камней.
    /// </summary>
    /// <param name="num">Количество камней.</param>
    public void SetGreenGemText(int num)
    {
        _greenGemText.SetText(num.ToString());
    }

    /// <summary>
    /// Установить текст синих камней.
    /// </summary>
    /// <param name="num">Количество камней.</param>
    public void SetBlueGemText(int num)
    {
        _blueGemText.SetText(num.ToString());
    }

    /// <summary>
    /// Установить текст золотых монет.
    /// </summary>
    /// <param name="num">Количество монет.</param>
    public void SetGoldText(int num)
    {
        _goldText.SetText(num.ToString());
    }
}
