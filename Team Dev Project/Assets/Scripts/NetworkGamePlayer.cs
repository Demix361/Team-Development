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

    public bool[] unlockedLevels = { true, false, false, false, false,
    false, false, false, false, false };//new bool[10];

    // Пройден ли текущий уровень
    [SerializeField]
    public bool levelCompleted;
    // ID текущего уровня
    [SerializeField]
    public int levelID;

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

    private void Update()
    {
        if (Input.GetButtonDown("Interact"))
        {
            Debug.Log($"levelID: {levelID}");
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

    public void SetLevelID(int id)
    {
        this.levelID = id;
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
