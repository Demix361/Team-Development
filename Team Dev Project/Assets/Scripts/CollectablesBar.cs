using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CollectablesBar : MonoBehaviour
{
    [SerializeField]
    private TMP_Text redGemText;
    [SerializeField]
    private TMP_Text greenGemText;
    [SerializeField]
    private TMP_Text blueGemText;
    [SerializeField]
    private TMP_Text goldText;

    public void SetRedGemText(int num)
    {
        redGemText.SetText(num.ToString());
    }

    public void SetGreenGemText(int num)
    {
        greenGemText.SetText(num.ToString());
    }

    public void SetBlueGemText(int num)
    {
        blueGemText.SetText(num.ToString());
    }

    public void SetGoldText(int num)
    {
        goldText.SetText(num.ToString());
    }
}
