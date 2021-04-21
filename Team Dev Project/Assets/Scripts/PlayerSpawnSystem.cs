using UnityEngine;
using Mirror;
using System.Collections.Generic;
using System.Linq;
/// <summary>
/// Класс системы появления игровых персонажей.
/// </summary>
public class PlayerSpawnSystem : NetworkBehaviour
{
    /// <summary>
    /// Prefab игрока.
    /// </summary>
    [SerializeField] private GameObject playerPrefab;
    /// <summary>
    /// Список мест появлений игроков.
    /// </summary>
    public static List<Transform> spawnPoints = new List<Transform>();
    /// <summary>
    /// Transform точки смерти персонажа.
    /// </summary>
    public static Transform deathPoint;
    /// <summary>
    /// Значение следующего индекса.
    /// </summary>
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
    /// <summary>
    /// Добавление точек появляние персонажей игроков.
    /// </summary>
    /// <param name="transform">Transform точки</param>
    public static void AddSpawnPoint(Transform transform)
    {
        spawnPoints.Add(transform);

        spawnPoints = spawnPoints.OrderBy(x => x.GetSiblingIndex()).ToList();
    }
    /// <summary>
    /// Удаление точек появление персонажей игроков.
    /// </summary>
    /// <param name="transform">Transform точки</param>
    public static void RemoveSpawnPoint(Transform transform)
    {
        spawnPoints.Remove(transform);
    }
    /// <summary>
    /// Добавление точек смерти персонажей игроков.
    /// </summary>
    /// <param name="transform">Transform точки</param>
    public static void AddDeathPoint(Transform transform)
    {
        deathPoint = transform;
    }
    /// <summary>
    /// Добавление функции обратного вызова.
    /// </summary>
    public override void OnStartServer()
    {
        MyNetworkManager.OnServerReadied += SpawnPlayer;
    }
    /// <summary>
    /// Удаление функции обратного вызова.
    /// </summary>
    [ServerCallback]
    private void OnDestory() => MyNetworkManager.OnServerReadied -= SpawnPlayer;
    /// <summary>
    /// Появление персонажей игроков.
    /// </summary>
    /// <param name="conn">Значение conn персонажа игрока</param>
    [Server]
    public void SpawnPlayer(NetworkConnection conn)
    {

        if (nextIndex >= Room.numPlayers)
        {
            nextIndex = 0;
        }

        Transform spawnPoint = spawnPoints.ElementAtOrDefault(nextIndex);

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
