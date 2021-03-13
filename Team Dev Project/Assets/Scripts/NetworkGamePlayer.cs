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

    // ID текущего уровня
    [SyncVar][SerializeField]
    public int levelID;
    private int levelAmount = 10;
    private int maxGemAmount = 20;

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

    public void SaveGems()
    {
        SaveSystem SS = new SaveSystem(displayName);
        GemData gemInfo = SS.LoadGems(levelID);
        bool[] newRedGem;
        bool[] newGreenGem;
        bool[] newBlueGem;
        Dictionary<string, bool[]> newGemInfo = new Dictionary<string, bool[]>();

        if (gemInfo != null)
        {
            newRedGem = gemInfo.redGems;
            newGreenGem = gemInfo.greenGems;
            newBlueGem = gemInfo.blueGems;
        }
        else
        {
            newRedGem = new bool[maxGemAmount];
            newGreenGem = new bool[maxGemAmount];
            newBlueGem = new bool[maxGemAmount];
        }

        // заполнение newGemInfo
        GameObject[] gems = GameObject.FindGameObjectsWithTag("Gem");
        for (int i = 0; i < gems.Length; i++)
        {
            Diamond gem = gems[i].GetComponent<Diamond>();

            if (gem.found)
            {
                if (gem.collectableName == "RedGem")
                {
                    newRedGem[gem.gemID] = true;
                }
                else if (gem.collectableName == "GreenGem")
                {
                    newGreenGem[gem.gemID] = true;
                }
                else if (gem.collectableName == "BlueGem")
                {
                    newBlueGem[gem.gemID] = true;
                }
            }
        }

        newGemInfo.Add("RedGem", newRedGem);
        newGemInfo.Add("GreenGem", newGreenGem);
        newGemInfo.Add("BlueGem", newBlueGem);

        SS.SaveGems(levelID, newGemInfo);

        Debug.Log($"[{displayName}] : GEMS SAVED");
    }

    public void SetGems()
    {
        GameObject[] gems = GameObject.FindGameObjectsWithTag("Gem");

        SaveSystem SS = new SaveSystem(displayName);

        GemData gemInfo = SS.LoadGems(levelID);
        if (gemInfo != null)
        {
            for (int i = 0; i < gems.Length; i++)
            {
                Diamond gem = gems[i].GetComponent<Diamond>();

                if (gem.collectableName == "RedGem")
                {
                    //gem.found = gemInfo.redGems[gem.gemID];
                    if (gemInfo.redGems[gem.gemID])
                    {
                        gem.SetGem();
                    }
                }
                else if (gem.collectableName == "GreenGem")
                {
                    //gem.found = gemInfo.greenGems[gem.gemID];
                    if (gemInfo.greenGems[gem.gemID])
                    {
                        gem.SetGem();
                    }
                }
                else if (gem.collectableName == "BlueGem")
                {
                    //gem.found = gemInfo.blueGems[gem.gemID];
                    if (gemInfo.blueGems[gem.gemID])
                    {
                        gem.SetGem();
                    }
                }
            }
        }

        Debug.Log($"[{displayName}] : GEMS SET");
    }

    [ClientRpc]
    public void RpcSaveLevel()
    {
        if (hasAuthority)
        {
            SaveLevel();
        }
    }

    [ClientRpc]
    public void RpcSaveGems()
    {
        if (hasAuthority)
        {
            SaveGems();
        }
    }
}
