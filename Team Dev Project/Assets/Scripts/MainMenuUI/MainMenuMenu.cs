using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuMenu : MonoBehaviour
{
    [SerializeField] private SteamLobby _steamLobby;

    private RoomsCanvases _roomsCanvases;

    public void FirstInitialize(RoomsCanvases canvases)
    {
        _roomsCanvases = canvases;
    }

    public void OnClick_CreateRoom()
    {
        _steamLobby.HostLobby();
    }


    public void OnClick_JoinRoom()
    {
        //_roomsCanvases.JoinRoomCanvas.Show();
    }

    public void OnClick_Options()
    {

    }

    public void OnClick_Credits()
    {

    }

    public void OnClick_Exit()
    {
        Application.Quit();
    }
}
