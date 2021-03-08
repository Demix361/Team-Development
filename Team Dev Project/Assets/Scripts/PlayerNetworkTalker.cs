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

    public void SetLevelId(int levelID)
    {
        foreach (var player in Room.GamePlayers)
        {
            if (player.hasAuthority)
            {
                player.levelID = levelID;
            }
        }
    }

    public void SetLevelComplete(bool state)
    {
        foreach (var player in Room.GamePlayers)
        {
            if (player.hasAuthority)
            {
                player.levelCompleted = state;
            }
        }
    }
}
