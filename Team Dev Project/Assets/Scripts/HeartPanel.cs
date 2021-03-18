﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class HeartPanel : NetworkBehaviour
{
    //private int heartAmount = 3;
    [SerializeField]
    private Sprite fullHeart;
    [SerializeField]
    private Sprite emptyHeart;
    [SerializeField]
    private Image[] heartImages = new Image[3];
    [SerializeField]
    private int maxHearts;

    [SyncVar(hook = nameof(UpdateHearts))]
    public int curHearts;


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
    }

    [Server]
    public void RemoveHeart()
    {
        curHearts -= 1;
    }

    [Server]
    public void RemoveAllHearts()
    {
        curHearts = 0;
    }

    [Server]
    public void AddAllHearts()
    {
        curHearts = maxHearts;
    }

}