using UnityEngine;

public class LevelExit : MonoBehaviour
{
    /// Объект экрана завершения игры
    [SerializeField] private GameObject endScreen;
    /// Объект всплывающего уведомления
    [SerializeField] private Popup popup;

    /// Объект общения с сервером
    private PlayerNetworkTalker playerNetworkTalker;
    /// Свойства игрока
    private PlayerProperties playerProperties;

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
    /// Метод смены сцены при нажатии кнопки
    public void ButtonOK()
    {   
        if (playerProperties)
        {
            playerProperties.allowInput = true;
        }
        playerNetworkTalker.CmdChangeScene("HubScene");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
            popup.SetPopup(true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
            popup.SetPopup(false);
    }
}
