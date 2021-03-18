using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System.Linq;

public class PlayerSpawnSystem : NetworkBehaviour
{
    [SerializeField] private GameObject playerPrefab = null;
    private static List<Transform> spawnPoints = new List<Transform>();
    public static Transform deathPoint;
    private int nextIndex = 0;

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

    public static void AddSpawnPoint(Transform transform)
    {
        spawnPoints.Add(transform);

        spawnPoints = spawnPoints.OrderBy(x => x.GetSiblingIndex()).ToList();
    }

    public static void RemoveSpawnPoint(Transform transform)
    {
        spawnPoints.Remove(transform);
    }

    public static void AddDeathPoint(Transform transform)
    {
        deathPoint = transform;
    }

    public override void OnStartServer()
    {
        MyNetworkManager.OnServerReadied += SpawnPlayer;
    }

    [ServerCallback]
    private void OnDestory() => MyNetworkManager.OnServerReadied -= SpawnPlayer;

    [Server]
    public void SpawnPlayer(NetworkConnection conn)
    {

        if (nextIndex >= Room.numPlayers)
        {
            nextIndex = 0;
        }

        Transform spawnPoint = spawnPoints.ElementAtOrDefault(nextIndex);

        var a = conn.clientOwnedObjects;

        Debug.Log(a);

        if (spawnPoint == null)
        {
            Debug.LogError($"Missing spawn point for player {nextIndex}");
            return;
        }

        GameObject playerInstance = Instantiate(playerPrefab, spawnPoints[nextIndex].position, spawnPoints[nextIndex].rotation);
        if (playerInstance.GetComponent<PlayerProperties>().playerId == -1)
        {
            playerInstance.GetComponent<PlayerProperties>().playerId = nextIndex;
        }
        //Debug.Log($"spawn_system: {playerInstance.GetComponent<PlayerProperties>().playerId}");

        NetworkServer.Spawn(playerInstance, conn);

        nextIndex++;
    }
}
