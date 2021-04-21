using Steamworks;
using UnityEngine;
using Mirror;

/// <summary>
/// Класс, отвечающий за взаимодействие с Steamworks
/// </summary>
public class SteamLobby : MonoBehaviour
{
    private const string HostAddressKey = "HostAddress";
    public static CSteamID LobbyId { get; private set; }

    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
    protected Callback<LobbyEnter_t> lobbyEntered;

    private MyNetworkManager room;
    private MyNetworkManager Room
    {
        get
        {
            if (room != null)
                return room;

            return room = NetworkManager.singleton as MyNetworkManager;
        }
    }

    /// <summary>
    /// Назначает функции обратного вызова.
    /// </summary>
    private void Start()
    {
        if (!SteamManager.Initialized)
            return;
        lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
        lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
    }

    /// <summary>
    /// Создает Steam-лобби.
    /// </summary>
    public void HostLobby()
    {
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, Room.maxConnections);
    }

    /// <summary>
    /// Вызовите, чтобы покинуть лобби.
    /// </summary>
    public void LeaveLobby()
    {
        if (LobbyId == null)
            return;

        SteamMatchmaking.LeaveLobby(LobbyId);
    }

    /// <summary>
    /// Вызывается после создания лобби. Запускает Host.
    /// </summary>
    /// <param name="callback">Callback типа LobbyCreated_t</param>
    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult != EResult.k_EResultOK)
        {
            Debug.Log("LOBBY FAILED TO CREATE");
            return;
        }

        LobbyId = new CSteamID(callback.m_ulSteamIDLobby);

        Room.StartHost();

        SteamMatchmaking.SetLobbyData(LobbyId, HostAddressKey, SteamUser.GetSteamID().ToString());
    }

    /// <summary>
    /// Вызывается после запроса на присоединение к лобби, присоединяет к лобби.
    /// </summary>
    /// <param name="callback">Callback типа GameLobbyJoinRequested_t</param>
    private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
    {
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    /// <summary>
    /// Вызывается после присоединения к лобби.
    /// </summary>
    /// <param name="callback">Callback типа LobbyEnter_t</param>
    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        if (NetworkServer.active)
            return;

        string hostAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey);

        Room.networkAddress = hostAddress;
        Room.StartClient();
        var roomsCanvases = FindObjectOfType<RoomsCanvases>();
        roomsCanvases.HideAll();
        roomsCanvases.MainMenuCanvas.Show();
    }
}
