using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.UI;


public class NetworkGamePlayer : NetworkBehaviour
{
    [SyncVar]
    public string displayName = "Loading...";

    [SyncVar]
    public int playerId = -1;

    // Player game properties
    [SerializeField] private Dictionary<string, int> collectables =
    new Dictionary<string, int>();

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

    public override void OnStartClient()
    {
        DontDestroyOnLoad(gameObject);

        Room.GamePlayers.Add(this);
    }

    public override void OnStopClient()
    {
        Room.GamePlayers.Remove(this);
    }

    [Server]
    public void SetDisplayName(string displayName)
    {
        this.displayName = displayName;
    }

    [Server]
    public void SetPlayerId(int id)
    {
        this.playerId = id;
    }

    public void IncreaseCollectable(string collName)
    {
        try
        {
            collectables[collName] += 1;
        }
        catch (KeyNotFoundException)
        {
            collectables.Add(collName, 1);
        }
        Debug.Log($"{collName}: {collectables[collName]}");
    }

    /*
    [Client]
    public void AddHealthBar()
    {
        Canvas canvas = GameObject.FindObjectOfType<Canvas>();

        for (int i = 0; i < Room.GamePlayers.Count; i++)
        {
            int pos_x = 380;
            int pos_y = 278;

            if (i == 0)
            {
                pos_x *= -1;
                pos_y *= -1;
            }
            else if (i == 1)
            {
                pos_y *= -1;
            }
            else if (i == 2)
            {
                pos_x *= -1;
            }

            Vector3 pos = new Vector3(pos_x, pos_y, 0);
            GameObject hb = Instantiate(Room.playerHealthBar, pos, Quaternion.identity) as GameObject;
            //hb.transform.parent = canvas.transform;
            hb.transform.SetParent(canvas.transform, false);

            Room.PlayerHealthBars.Add(hb);
            //NetworkServer.Spawn(hb);
        }
    }
    */
}
