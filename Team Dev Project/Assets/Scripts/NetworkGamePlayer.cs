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

    //[SyncVar]
    //public bool[] unlockedLevels = { true, false, false, false, false, false, false, false, false, false };
    //public SyncList<bool> unlockedLevels = new SyncList<bool>() { true, false, false, false, false, false, false, false, false, false };

    // ID текущего уровня
    [SyncVar][SerializeField]
    public int levelID;
    private int levelAmount = 10;

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
        if (Input.GetButtonDown("DevButton"))
        {
            CmdChangeScene("HubScene");
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

    [Server]
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


    [Command]
    public void CmdChangeScene(string sceneName)
    {
        Room.ServerChangeScene(sceneName);
    }

    public void SaveLevel()
    {
        SaveSystem SS = new SaveSystem(displayName);
        GameData levelInfo = SS.LoadGame();
        bool[] newLevelInfo;

        if (levelInfo != null)
        {
            newLevelInfo = levelInfo.unlockedLevels;
        }
        else
        {
            newLevelInfo = new bool[levelAmount];
        }

        for (int i = 0; i <= levelID + 1 && i + 1 < levelAmount; i++)
        {
            newLevelInfo[i] = true;
        }

        SS.SaveGame(newLevelInfo);

        Debug.Log($"[{displayName}] : LEVELS SAVED");
    }

    public void UnlockDoors()
    {
        GameObject[] doors = GameObject.FindGameObjectsWithTag("Door");

        SaveSystem SS = new SaveSystem(displayName);

        GameData levelInfo = SS.LoadGame();
        if (levelInfo != null)
        {
            for (int i = 0; i < doors.Length; i++)
            {
                Door door = doors[i].GetComponent<Door>();
                door.SetLock(!levelInfo.unlockedLevels[door.doorID]);
            }
        }

        Debug.Log($"[{displayName}] : DOORS UNLOCKED");
    }

    [ClientRpc]
    public void RpcSaveLevel()
    {
        if (hasAuthority)
        {
            SaveLevel();
        }
    }
}
