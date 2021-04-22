using UnityEngine;

/// <summary>
/// Класс информации об уровне.
/// </summary>
public class DoorInfoPlate : MonoBehaviour
{
    /// <summary>
    /// Объект двери.
    /// </summary>
    [SerializeField] private Door door;
    /// <summary>
    /// UI окна статистики уровня.
    /// </summary>
    [SerializeField] private GameObject levelStatsUI;
    /// <summary>
    /// Объект всплывающего уведомления.
    /// </summary>
    [SerializeField] private Popup popup;
    /// <summary>
    /// Объект свойств пользователя.
    /// </summary>
    private PlayerProperties playerProperties;


    /// <summary>
    /// Открывает окно статистики уровня при нахождении игрока в триггере и нажатии кнопки взаимодействия.
    /// </summary>
    /// <param name="collision">Collider2D объекта, находящегося в триггере.</param>
    private void OnTriggerStay2D(Collider2D collision)
    {
        playerProperties = collision.GetComponent<PlayerProperties>();

        if (playerProperties.allowInput && (Input.GetButtonDown("Interact") || Input.GetButton("Interact")))
        {
            playerProperties.allowInput = false;

            if (collision.GetComponent<PlayerNetworkTalker>().hasAuthority)
            {
                levelStatsUI.GetComponent<LevelStats>().SetLevelName($"Level {door.doorID + 1}");
                levelStatsUI.GetComponent<LevelStats>().SetGems(door.doorID, collision.GetComponent<PlayerNetworkTalker>().getPlayerName());
                levelStatsUI.SetActive(true);
            }
        }
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

    /// <summary>
    /// Закрывает окно статистики уровня.
    /// </summary>
    public void Close()
    {
        levelStatsUI.SetActive(false);
        playerProperties.allowInput = true;
    }
}
