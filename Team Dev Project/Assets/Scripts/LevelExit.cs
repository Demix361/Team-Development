using UnityEngine;

/// <summary>
/// Класс объекта выхода с уровня.
/// </summary>
public class LevelExit : MonoBehaviour
{
    /// <summary>
    /// Объект экрана завершения игры.
    /// </summary>
    [SerializeField] private GameObject endScreen;
    /// <summary>
    /// Объект всплывающего уведомления.
    /// </summary>
    [SerializeField] private Popup popup;

    /// <summary>
    /// Объект общения с сервером.
    /// </summary>
    private PlayerNetworkTalker playerNetworkTalker;
    /// <summary>
    /// Свойства игрока.
    /// </summary>
    private PlayerProperties playerProperties;

    /// <summary>
    /// Включение экрана прохождения уровня.
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (Input.GetButton("Interact"))
        {
            playerNetworkTalker = collision.GetComponent<PlayerNetworkTalker>();
            playerProperties = collision.GetComponent<PlayerProperties>();

            playerProperties.allowInput = false;

            endScreen.SetActive(true);

            // Сохранение
            playerNetworkTalker.CmdSaveLevel();
            playerNetworkTalker.CmdSaveGems();
        }
    }

    /// <summary>
    /// Метод смены сцены при нажатии кнопки.
    /// </summary>
    public void ButtonOK()
    {   
        if (playerProperties)
        {
            playerProperties.allowInput = true;
        }
        playerNetworkTalker.CmdChangeScene("HubScene");
    }

    /// <summary>
    /// Активирует всплывающее уведомление, при вхождении игрока в триггер.
    /// </summary>
    /// <param name="collision">Collider2D объекта, вошедшего в триггер.</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
            popup.SetPopup(true);
    }

    /// <summary>
    /// Деактивирует всплывающее уведомление, при выходе игрока из триггера.
    /// </summary>
    /// <param name="collision">Collider2D объекта, вышедшего в триггер.</param>
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
            popup.SetPopup(false);
    }
}
