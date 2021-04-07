using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class HeartPanel : NetworkBehaviour
{
    [SerializeField] private Sprite fullHeart;
    [SerializeField] private Sprite emptyHeart;
    [SerializeField] private GameObject container;
    [SerializeField] private SpectatorMode spectatorMode;
    [SerializeField] private GameObject heartObject;
    [SerializeField] private int heartPerPlayer;
    private List<Image> heartImages = new List<Image>();
    private int maxHearts;
    [SyncVar(hook = nameof(UpdateHearts))] public int curHearts;

    private MyNetworkManager room;
    private MyNetworkManager Room
    {
        get
        {
            if (room != null)
            {
                return room;
            }

            return room = NetworkManager.singleton as MyNetworkManager;
        }
    }


    private void Start()
    {
        maxHearts = Room.GamePlayers.Count * heartPerPlayer;
        float size = heartObject.GetComponent<RectTransform>().rect.width + 8;
        container.GetComponent<RectTransform>().sizeDelta = new Vector2(size * maxHearts - 2, size);

        for (int i = 0; i < maxHearts; i++)
        {
            heartImages.Add(GameObject.Instantiate(heartObject,container.transform).GetComponent<Image>());
            heartImages[i].GetComponent<RectTransform>().anchoredPosition = new Vector3(size * i, 0, 0);
        }
    }

    [ClientRpc]
    private void SetReviveButton(bool state)
    {
        spectatorMode.reviveButton.interactable = state;
    }

    // hook
    private void UpdateHearts(int oldValue, int newValue)
    {
        for (int i = 0; i < heartImages.Count; i++)
        {
            if (i < curHearts)
            {
                heartImages[i].sprite = fullHeart;
            }
            else
            {
                heartImages[i].sprite = emptyHeart;
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

    public bool IsMaxHearts()
    {
        if (curHearts == maxHearts)
        {
            return true;
        }
        return false;
    }

}
