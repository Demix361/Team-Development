using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class HeartPanel : NetworkBehaviour
{
    [SerializeField]
    private Sprite fullHeart;
    [SerializeField]
    private Sprite emptyHeart;
    [SerializeField]
    private Image[] heartImages = new Image[3];
    [SerializeField]
    private int maxHearts;
    [SerializeField]
    private SpectatorMode spectatorMode;

    [SyncVar(hook = nameof(UpdateHearts))]
    public int curHearts;

    [ClientRpc]
    private void SetReviveButton(bool state)
    {
        spectatorMode.reviveButton.interactable = state;
    }

    // hook
    private void UpdateHearts(int oldValue, int newValue)
    {
        for (int i = 0; i < heartImages.Length; i++)
        {
            if (i < curHearts)
            {
                heartImages[i].sprite = fullHeart;
                //heartAnimators[i].SetBool("State", true);
            }
            else
            {
                heartImages[i].sprite = emptyHeart;
                //heartAnimators[i].SetBool("State", false);
            }
        }
    }

    [Server]
    public void AddHeart()
    {
        curHearts += 1;
        SetReviveButton(true);
        //spectatorMode.reviveButton.interactable = true;
    }

    [Server]
    public void RemoveHeart()
    {
        curHearts -= 1;

        if (curHearts < 0)
        {
            curHearts = 0;
        }

        if (curHearts == 0)
        {
            SetReviveButton(false);
            //spectatorMode.reviveButton.interactable = false;
        }
    }

    [Server]
    public void RemoveAllHearts()
    {
        curHearts = 0;
        SetReviveButton(false);
    }

    [Server]
    public void AddAllHearts()
    {
        curHearts = maxHearts;
        SetReviveButton(true);
        //spectatorMode.reviveButton.interactable = true;
    }

}
