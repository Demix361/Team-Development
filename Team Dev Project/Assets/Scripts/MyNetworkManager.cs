using UnityEngine;
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

    public override void OnServerSceneChanged(string sceneName)
    {
        if (sceneName.StartsWith("HubScene"))
        {
            //GameObject[] doors = GameObject.FindGameObjectsWithTag("Door");
            //Debug.Log($"door num: {doors.Length}");


        }

        
        if (sceneName.StartsWith("LevelScene"))
        {

        }
        
    }

    [Client]
    public override void OnClientSceneChanged(NetworkConnection conn)
    {
        // Сохранение после завершения уровня
        if (SceneManager.GetActiveScene().name.StartsWith("HubScene"))
        {
            GameObject[] doors = GameObject.FindGameObjectsWithTag("Door");
            Debug.Log($"door num: {doors.Length}");

            

            foreach (NetworkGamePlayer player in GamePlayers)
            {
                if (player.connectionToServer == conn)
                {
                    SaveSystem SS = new SaveSystem(player.displayName);

                    if (player.levelID == -2)
                    {
                        Debug.Log("LOAD LEVELS");
                        
                        GameData levelInfo = SS.LoadGame();
                        if (levelInfo != null)
                        {
                            Debug.Log("not null");

                            for (int i = 0; i < levelInfo.unlockedLevels.Count(); i++)
                            {
                                player.unlockedLevels[i] = levelInfo.unlockedLevels[i];
                            }

                            for (int i = 0; i < player.unlockedLevels.Count(); i++)
                            {
                                Debug.Log($"player.unlockedlevels: {player.unlockedLevels[i]}");
                            }
                            //player.unlockedLevels = levelInfo.unlockedLevels;
                        }
                        
                    }

                    if (player.levelCompleted)
                    {
                        Debug.Log("SAVED");
                        player.levelCompleted = false;

                        for (int i = 0; i <= player.levelID; i++)
                        {
                            player.unlockedLevels[i] = true;
                        }

                        if (player.levelID + 1 != player.unlockedLevels.Length && player.levelID >= 0)
                        {
                            player.unlockedLevels[player.levelID + 1] = true;
                        }

                        for (int i = 0; i < doors.Length; i++)
                        {
                            Door door = doors[i].GetComponent<Door>();
                            door.SetLock(!player.unlockedLevels[door.doorID]);
                        }

                        SS.SaveGame(player.unlockedLevels);
                        
                    }

                    
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
}
