using UnityEngine;

/// <summary>
/// Класс холста настроек.
/// </summary>
public class SettingsCanvas : MonoBehaviour
{
    /// <summary>
    /// Объект меню настроек.
    /// </summary>
    [SerializeField] private SettingsMenu _settingsMenu;
    /// <summary>
    /// Объект холстов меню.
    /// </summary>
    private RoomsCanvases _roomsCanvases;

    /// <summary>
    /// Инициализация полей класса.
    /// </summary>
    /// <param name="canvases">Объект RoomsCanvases</param>
    public void FirstInitialize(RoomsCanvases canvases)
    {
        _roomsCanvases = canvases;
        _settingsMenu.FirstInitialize(canvases);
    }

    /// <summary>
    /// Показать объект.
    /// </summary>
    public void Show()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Скрыть объект.
    /// </summary>
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
