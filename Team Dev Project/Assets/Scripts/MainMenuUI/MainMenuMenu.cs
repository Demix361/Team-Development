using UnityEngine;
using Mirror.FizzySteam;
using System.IO;

/// <summary>
/// Класс главного меню.
/// </summary>
public class MainMenuMenu : MonoBehaviour
{
    /// <summary>
    /// Объект SteamLobby.
    /// </summary>
    [SerializeField] private SteamLobby _steamLobby;

    /// <summary>
    /// Объект холстов меню.
    /// </summary>
    private RoomsCanvases _roomsCanvases;
    /// <summary>
    /// Путь к файлам сохранений.
    /// </summary>
    private static string path;


    /// <summary>
    /// Инициализация полей класса.
    /// </summary>
    /// <param name="canvases">Объект RoomsCanvases</param>
    public void FirstInitialize(RoomsCanvases canvases)
    {
        _roomsCanvases = canvases;
    }

    /// <summary>
    /// Кнопка создания комнаты.
    /// </summary>
    public void OnClick_CreateRoom()
    {
        FindObjectOfType<FizzySteamworks>().enabled = true;
        _steamLobby.HostLobby();

        _roomsCanvases.HideAll();
        _roomsCanvases.MainMenuCanvas.Show();
    }

    /// <summary>
    /// Кнопка вызова меню настроек.
    /// </summary>
    public void OnClick_Options()
    {
        _roomsCanvases.MainMenuCanvas.Hide();
        _roomsCanvases.SettingsCanvas.Show();
    }

    /// <summary>
    /// Кнопка вызова титров.
    /// </summary>
    public void OnClick_Credits()
    {

    }

    /// <summary>
    /// Кнопка закрытия приложения.
    /// </summary>
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
