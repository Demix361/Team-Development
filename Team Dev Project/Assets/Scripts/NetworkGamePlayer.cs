using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.UI;


public class NetworkGamePlayer : NetworkBehaviour
{
    [SyncVar]
    private string displayName = "Loading...";

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
}
