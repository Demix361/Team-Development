using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SteamLobby : MonoBehaviour
{
    [SerializeField] private MyNetworkManager _networkManager;
    [SerializeField] private RoomsCanvases _roomsCanvases;

    private const string HostAddressKey = "HostAddress";
    public static CSteamID LobbyId { get; private set; }

    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
    protected Callback<LobbyEnter_t> lobbyEntered;

    private void Start()
    {
        if (!SteamManager.Initialized)
            return;
        lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
        lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
    }

    public void HostLobby()
    {
        SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, _networkManager.maxConnections);
    }

    public void LeaveLobby()
    {
        if (LobbyId == null)
            return;

        SteamMatchmaking.LeaveLobby(LobbyId);
        Debug.Log("Left Lobby");
    }

    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult != EResult.k_EResultOK)
        {
            Debug.Log("LOBBY FAILED TO CREATE");
            return;
        }

        LobbyId = new CSteamID(callback.m_ulSteamIDLobby);

        _networkManager.StartHost();

        SteamMatchmaking.SetLobbyData(LobbyId, HostAddressKey, SteamUser.GetSteamID().ToString());

        //_roomsCanvases.CurrentRoomCanvas.Show();
    }

    private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
    {
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        if (NetworkServer.active)
            return;

        string hostAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey);

        _networkManager.networkAddress = hostAddress;
        _networkManager.StartClient();
        _roomsCanvases.HideAll();
        _roomsCanvases.MainMenuCanvas.Show();
    }
}
