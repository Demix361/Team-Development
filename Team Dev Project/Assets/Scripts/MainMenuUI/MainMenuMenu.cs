using UnityEngine;
using Mirror.FizzySteam;

/// <summary> Класс главного меню. </summary>
public class MainMenuMenu : MonoBehaviour
{
    /// <summary> Объект SteamLobby. </summary>
    [SerializeField] private SteamLobby _steamLobby;

    /// <summary> Объект холстов меню. </summary>
    private RoomsCanvases _roomsCanvases;

    /// <summary> Инициализация полей класса. </summary>
    /// <param name="canvases">Объект RoomsCanvases</param>
    public void FirstInitialize(RoomsCanvases canvases)
    {
        _roomsCanvases = canvases;
    }

    /// <summary> Кнопка создания комнаты. </summary>
    public void OnClick_CreateRoom()
    {
        FindObjectOfType<FizzySteamworks>().enabled = true;
        _steamLobby.HostLobby();

        _roomsCanvases.HideAll();
        _roomsCanvases.MainMenuCanvas.Show();
    }

    /// <summary> Кнопка вызова меню настроек. </summary>
    public void OnClick_Options()
    {
        _roomsCanvases.MainMenuCanvas.Hide();
        _roomsCanvases.SettingsCanvas.Show();
    }

    /// <summary> Кнопка вызова титров. </summary>
    public void OnClick_Credits()
    {
        _roomsCanvases.MainMenuCanvas.Hide();
        _roomsCanvases.CreditsCanvas.Show();
    }

    /// <summary> Кнопка закрытия приложения. </summary>
    public void OnClick_Exit()
    {
        Application.Quit();
    }
}
