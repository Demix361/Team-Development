using System.Collections.Generic;
using UnityEngine;
using Mirror;

/// <summary>
/// Класс игрока в игре.
/// </summary>
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
    }

    public void SaveGems()
    {
        SaveSystem SS = new SaveSystem(displayName);
        GemData gemInfo = SS.LoadGems(levelID);
        List<bool> newRedGem;
        List<bool> newGreenGem;
        List<bool> newBlueGem;
        Dictionary<string, List<bool>> newGemInfo = new Dictionary<string, List<bool>>();

        CollectablesData collData = SS.LoadCollectables();

        GameObject[] gems = GameObject.FindGameObjectsWithTag("Gem");

        // Сортировка gems по transform.position
        for (int i = 0; i < gems.Length - 1; i++)
        {
            for (int j = 0; j < gems.Length - i - 1; j++)
            {
                if (gems[j + 1].transform.position.x < gems[j].transform.position.x)
                {
                    var temp = gems[j + 1];
                    gems[j + 1] = gems[j];
                    gems[j] = temp;
                }
                else if (gems[j + 1].transform.position.x == gems[j].transform.position.x && gems[j + 1].transform.position.y < gems[j].transform.position.y)
                {
                    var temp = gems[j + 1];
                    gems[j + 1] = gems[j];
                    gems[j] = temp;
                }
            }
        }

        if (gemInfo != null)
        {
            newRedGem = gemInfo.redGems;
            newGreenGem = gemInfo.greenGems;
            newBlueGem = gemInfo.blueGems;
        }
        else
        {
            newRedGem = new List<bool>();
            newGreenGem = new List<bool>();
            newBlueGem = new List<bool>();

            for (int i = 0; i < gems.Length; i++)
            {
                Gem gem = gems[i].GetComponent<Gem>();
                if (gem.collectableName == "RedGem")
                {
                    newRedGem.Add(false);
                }
                else if (gem.collectableName == "GreenGem")
                {
                    newGreenGem.Add(false);
                }
                else if (gem.collectableName == "BlueGem")
                {
                    newBlueGem.Add(false);
                }
            }
        }

        if (collData == null)
        {
            collData = new CollectablesData(0, 0, 0, 0);
        }

        // заполнение newGemInfo
        int r_i = 0, g_i = 0, b_i = 0;
        for (int i = 0; i < gems.Length; i++)
        {
            Gem gem = gems[i].GetComponent<Gem>();

            if (gem.collectableName == "RedGem")
            {
                if (gem.found)
                {
                    if (newRedGem[r_i] == false)
                        collData.redGemNum += 1;
                    newRedGem[r_i] = true;
                }
                r_i += 1;
            }
            else if (gem.collectableName == "GreenGem")
            {
                if (gem.found)
                {
                    if (newGreenGem[g_i] == false)
                        collData.greenGemNum += 1;
                    newGreenGem[g_i] = true;
                }
                g_i += 1;
            }
            else if (gem.collectableName == "BlueGem")
            {
                if (gem.found)
                {
                    if (newBlueGem[b_i] == false)
                        collData.blueGemNum += 1;
                    newBlueGem[b_i] = true;
                }
                b_i += 1;
            }
        }

        newGemInfo.Add("RedGem", newRedGem);
        newGemInfo.Add("GreenGem", newGreenGem);
        newGemInfo.Add("BlueGem", newBlueGem);

        SS.SaveGems(levelID, newGemInfo);

        SS.SaveCollectables(collData);
    }

    public void SetGems()
    {
        GameObject[] gems = GameObject.FindGameObjectsWithTag("Gem");
        // Сортировка gems по transform.position
        for (int i = 0; i < gems.Length - 1; i++)
        {
            for (int j = 0; j < gems.Length - i - 1; j++)
            {
                if (gems[j + 1].transform.position.x < gems[j].transform.position.x)
                {
                    var temp = gems[j + 1];
                    gems[j + 1] = gems[j];
                    gems[j] = temp;
                }
                else if (gems[j + 1].transform.position.x == gems[j].transform.position.x && gems[j + 1].transform.position.y < gems[j].transform.position.y)
                {
                    var temp = gems[j + 1];
                    gems[j + 1] = gems[j];
                    gems[j] = temp;
                }
            }
        }

        SaveSystem SS = new SaveSystem(displayName);

        GemData gemInfo = SS.LoadGems(levelID);
        int r_i = 0, g_i = 0, b_i = 0;

        if (gemInfo != null)
        {
            for (int i = 0; i < gems.Length; i++)
            {
                Gem gem = gems[i].GetComponent<Gem>();

                if (gem.collectableName == "RedGem")
                {
                    if (gemInfo.redGems[r_i])
                    {
                        gem.SetGem();
                    }
                    r_i += 1;
                }
                else if (gem.collectableName == "GreenGem")
                {
                    if (gemInfo.greenGems[g_i])
                    {
                        gem.SetGem();
                    }
                    g_i += 1;
                }
                else if (gem.collectableName == "BlueGem")
                {
                    if (gemInfo.blueGems[b_i])
                    {
                        gem.SetGem();
                    }
                    b_i += 1;
                }
            }
        }
    }

    public void UpdateCollectables()
    {
        SaveSystem SS = new SaveSystem(displayName);

        CollectablesData collData = SS.LoadCollectables();

        if (collData != null)
        {
            CollectablesBar collBar = GameObject.Find("CanvasCollectables").GetComponent<CollectablesBar>();

            collBar.SetRedGemText(collData.redGemNum);
            collBar.SetGreenGemText(collData.greenGemNum);
            collBar.SetBlueGemText(collData.blueGemNum);
            collBar.SetGoldText(collData.goldNum);
        }
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
