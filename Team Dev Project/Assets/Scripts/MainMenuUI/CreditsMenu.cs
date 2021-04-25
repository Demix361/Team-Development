using UnityEngine;

/// <summary> Класс меню титров. </summary>
public class CreditsMenu : MonoBehaviour
{
    /// <summary> Объект холстов меню. </summary>
    private RoomsCanvases _roomsCanvases;

    /// <summary> Инициализация полей класса. </summary>
    /// <param name="canvases">Объект RoomsCanvases</param>
    public void FirstInitialize(RoomsCanvases canvases)
    {
        _roomsCanvases = canvases;
    }

    /// <summary> Закрытие меню настроек. </summary>
    public void OnClick_Return()
    {
        _roomsCanvases.CreditsCanvas.Hide();
        _roomsCanvases.MainMenuCanvas.Show();
    }
}
