using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelStats : MonoBehaviour
{
    [SerializeField]
    private TMP_Text levelName;
    [SerializeField]
    private TMP_Text redGemsText;
    [SerializeField]
    private TMP_Text greenGemsText;
    [SerializeField]
    private TMP_Text blueGemsText;

    public void SetLevelName(string levelName)
    {
        this.levelName.SetText(levelName);
    }

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
