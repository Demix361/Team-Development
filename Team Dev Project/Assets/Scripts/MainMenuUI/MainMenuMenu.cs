using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror.FizzySteam;
using System.IO;

public class MainMenuMenu : MonoBehaviour
{
    [SerializeField] private SteamLobby _steamLobby;

    private RoomsCanvases _roomsCanvases;
    private static string path;

    public void FirstInitialize(RoomsCanvases canvases)
    {
        _roomsCanvases = canvases;
    }

    public void OnClick_CreateRoom()
    {
        FindObjectOfType<FizzySteamworks>().enabled = true;
        _steamLobby.HostLobby();

        _roomsCanvases.HideAll();
        _roomsCanvases.MainMenuCanvas.Show();
    }

    public void OnClick_Options()
    {
        _roomsCanvases.MainMenuCanvas.Hide();
        _roomsCanvases.SettingsCanvas.Show();
    }

    public void OnClick_Credits()
    {

    }

    public void OnClick_Exit()
    {
        Application.Quit();
    }

    public void OnClick_DeleteAllSaves()
    {
        path = Application.persistentDataPath;

        DirectoryInfo di = new DirectoryInfo(path);

        foreach (FileInfo file in di.GetFiles())
        {
            file.Delete();
        }
        foreach (DirectoryInfo dir in di.GetDirectories())
        {
            dir.Delete(true);
        }
    }
}
