using UnityEngine;

/// <summary>
/// Класс холста главного меню.
/// </summary>
public class MainMenuCanvas : MonoBehaviour
{
    /// <summary>
    /// Объект меню главного меню.
    /// </summary>
    [SerializeField] private MainMenuMenu _mainMenuMenu;
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
        _mainMenuMenu.FirstInitialize(canvases);
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
