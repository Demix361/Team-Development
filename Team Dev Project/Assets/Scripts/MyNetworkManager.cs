﻿using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;

public class MyNetworkManager : NetworkManager
{
    [SerializeField] private int minPlayers = 2;
    [Scene] [SerializeField] private string menuScene = string.Empty;

    [Header("Room")]
    [SerializeField] private NetworkRoomPlayer roomPlayerPrefab = null;

    [Header("Game")]
    [SerializeField] private NetworkGamePlayer gamePlayerPrefab = null;
    [SerializeField] private GameObject playerSpawnSystem = null;
    [SerializeField] public GameObject playerHealthBar = null;

    public static event Action OnClientConnected;
    public static event Action OnClientDisconnected;
    public static event Action OnServerDisconnected;
    public static event Action<NetworkConnection> OnServerReadied;

    public List<NetworkRoomPlayer> RoomPlayers { get; } = new List<NetworkRoomPlayer>();
    public List<NetworkGamePlayer> GamePlayers { get; } = new List<NetworkGamePlayer>();
    public List<GameObject> PlayerHealthBars { get; } = new List<GameObject>();

    public override void OnStartServer()
    {
        spawnPrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs").ToList();
    }

    public override void OnStartClient()
    {
        var spawnablePrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs");

        foreach (var prefab in spawnablePrefabs)
        {
            ClientScene.RegisterPrefab(prefab);
        }
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);

        OnClientConnected?.Invoke();
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);

        OnClientDisconnected?.Invoke();
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        if (numPlayers >= maxConnections)
        {
            conn.Disconnect();
            return;
        }

        //if (SceneManager.GetActiveScene().name != menuScene)
        if (SceneManager.GetActiveScene().path != menuScene)
        {
            conn.Disconnect();
            return;
        }
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        //if (SceneManager.GetActiveScene().name == menuScene)
        if (SceneManager.GetActiveScene().path == menuScene)
        {
            bool isLeader = RoomPlayers.Count == 0;

            NetworkRoomPlayer roomPlayerInstance = Instantiate(roomPlayerPrefab);

            roomPlayerInstance.IsLeader = isLeader;

            NetworkServer.AddPlayerForConnection(conn, roomPlayerInstance.gameObject);
        }
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        if (conn.identity != null)
        {
            var player = conn.identity.GetComponent<NetworkRoomPlayer>();

            RoomPlayers.Remove(player);

            NotifyPlayersOfReadyState();
        }

        base.OnServerDisconnect(conn);
    }

    public override void OnStopServer()
    {
        RoomPlayers.Clear();

        OnServerDisconnected?.Invoke();

        Destroy(gameObject);
    }

    public void NotifyPlayersOfReadyState()
    {
        foreach (var player in RoomPlayers)
        {
            player.HandleReadyToStart(IsReadyToStart());
        }
    }

    private bool IsReadyToStart()
    {
        if (numPlayers < minPlayers)
        {
            return false;
        }

        foreach (var player in RoomPlayers)
        {
            if (!player.IsReady)
            {
                return false;
            }
        }

        return true;
    }

    public void StartGame()
    {
        if (SceneManager.GetActiveScene().path == menuScene)
        {
            if (!IsReadyToStart())
            {
                return;
            }

            ServerChangeScene("HubScene");
        }
    }

    public override void ServerChangeScene(string newSceneName)
    {
        if (SceneManager.GetActiveScene().path == menuScene && (newSceneName.StartsWith("HubScene")))// || newSceneName.StartsWith("LevelScene")))
        {
            for (int i = RoomPlayers.Count - 1; i >= 0; i--)
            {
                var conn = RoomPlayers[i].connectionToClient;
                var gameplayerInstance = Instantiate(gamePlayerPrefab);
                gameplayerInstance.SetDisplayName(RoomPlayers[i].DisplayName);
                

                NetworkServer.Destroy(conn.identity.gameObject);

                gameplayerInstance.SetPlayerId(i + 1);

                NetworkServer.ReplacePlayerForConnection(conn, gameplayerInstance.gameObject);
            }

            GameObject playerSpawnSystemInstance = Instantiate(playerSpawnSystem);
            NetworkServer.Spawn(playerSpawnSystemInstance);
        }

        base.ServerChangeScene(newSceneName);
    }

    /*
    public override void OnServerSceneChanged(string sceneName)
    {
        if (sceneName.StartsWith("HubScene"))
        {
            foreach (NetworkGamePlayer player in GamePlayers)
            {
                player.UnlockDoors();
            }
        }    
    }
    */

    public override void OnClientSceneChanged(NetworkConnection conn)
    {
        if (SceneManager.GetActiveScene().name.StartsWith("HubScene"))
        {
            foreach (NetworkGamePlayer player in GamePlayers)
            {
                if (player.hasAuthority)
                {
                    player.UnlockDoors();
                }
            }
        }

        if (SceneManager.GetActiveScene().name.StartsWith("LevelScene"))
        {
            foreach (NetworkGamePlayer player in GamePlayers)
            {
                if (player.hasAuthority)
                {
                    player.SetGems();
                }
            }
        }

        base.OnClientSceneChanged(conn);
    }

    public override void OnServerReady(NetworkConnection conn)
    {
        base.OnServerReady(conn);

        OnServerReadied?.Invoke(conn);
    }

    public void ServerIncreaseCollectable(string collName)
    {
        foreach (NetworkGamePlayer player in GamePlayers)
        {
            player.IncreaseCollectable(collName);
        }
    }

    public void ServerSetLevelID(int levelID)
    {
        foreach (NetworkGamePlayer player in GamePlayers)
        {
            player.SetLevelID(levelID);
        }
    }

    public void ServerSaveLevel()
    {
        foreach (NetworkGamePlayer player in GamePlayers)
        {
            player.RpcSaveLevel();
        }
    }

    public void ServerSaveGems()
    {
        foreach (NetworkGamePlayer player in GamePlayers)
        {
            player.RpcSaveGems();
        }
    }
}
