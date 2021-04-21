using UnityEngine;
using TMPro;

/// <summary>
/// Класс статистики уровня.
/// </summary>
public class LevelStats : MonoBehaviour
{
    /// <summary>
    /// Текстовое поле названия уровня.
    /// </summary>
    [SerializeField]
    private TMP_Text levelName;
    /// <summary>
    /// Текстовое поле красных камней.
    /// </summary>
    [SerializeField]
    private TMP_Text redGemsText;
    /// <summary>
    /// Текстовое поле зеленых камней.
    /// </summary>
    [SerializeField]
    private TMP_Text greenGemsText;
    /// <summary>
    /// Текстовое поле синих камней.
    /// </summary>
    [SerializeField]
    private TMP_Text blueGemsText;

    /// <summary>
    /// Метод установки имени уровня.
    /// </summary>
    public void SetLevelName(string levelName)
    {
        this.levelName.SetText(levelName);
    }

    /// <summary>
    /// Метод установки камней.
    /// </summary>
    public void SetGems(int levelID, string playerName)
    {
        SaveSystem SS = new SaveSystem(playerName);
        GemData gems = SS.LoadGems(levelID);
        int counter = 0;

        if (gems != null)
        {
            foreach(var g in gems.redGems)
            {
                if (g)
                {
                    counter++;
                }
            }
            redGemsText.SetText($"{counter} / {gems.redGems.Count}");

            counter = 0;
            foreach (var g in gems.greenGems)
            {
                if (g)
                {
                    counter++;
                }
            }
            greenGemsText.SetText($"{counter} / {gems.greenGems.Count}");

            counter = 0;
            foreach (var g in gems.blueGems)
            {
                if (g)
                {
                    counter++;
                }
            }
            blueGemsText.SetText($"{counter} / {gems.blueGems.Count}");
        }
        else
        {
            redGemsText.SetText("? / ?");
            greenGemsText.SetText("? / ?");
            blueGemsText.SetText("? / ?");
        }
    }
}
