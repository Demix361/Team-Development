using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerNetworkTalker : NetworkBehaviour
{
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

    [Command]
    public void CmdChangeScene(string sceneName)
    {
        Room.ServerChangeScene(sceneName);
    }

    [Command]
    public void CmdIncreaseCollectable(string collName)
    {
        Room.ServerIncreaseCollectable(collName);
    }

    [Command]
    public void CmdSetLevelID(int levelID)
    {
        Room.ServerSetLevelID(levelID);
    }

    [Command]
    public void CmdSaveLevel()
    {
        Room.ServerSaveLevel();
    }

    [Command]
    public void CmdSaveGems()
    {
        Room.ServerSaveGems();
    }
}
