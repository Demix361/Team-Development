using UnityEngine;

/// <summary> Класс холста титров. </summary>
public class CreditsCanvas : MonoBehaviour
{
    /// <summary> Объект меню настроек. </summary>
    [SerializeField] private CreditsMenu _creditsMenu;

    /// <summary> Объект холстов меню. </summary>
    private RoomsCanvases _roomsCanvases;

    /// <summary> Инициализация полей класса. </summary>
    /// <param name="canvases">Объект RoomsCanvases</param>
    public void FirstInitialize(RoomsCanvases canvases)
    {
        _roomsCanvases = canvases;
        _creditsMenu.FirstInitialize(canvases);
    }

    /// <summary> Показать объект. </summary>
    public void Show()
    {
        gameObject.SetActive(true);
    }

    /// <summary> Скрыть объект. </summary>
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
