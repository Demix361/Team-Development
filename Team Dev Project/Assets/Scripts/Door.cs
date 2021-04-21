using UnityEngine;
using TMPro;

/// <summary>
/// Класс двери.
/// </summary>
/// <remarks>
/// При активации меняет текущую сцену на заданную.
/// </remarks>
public class Door : MonoBehaviour
{
    /// <summary>
    /// Название новой сцены.
    /// </summary>
    [SerializeField] public string sceneName;
    /// <summary>
    /// Название уровня.
    /// </summary>
    [SerializeField] private string tableString;
    /// <summary>
    /// Поле названия уровня.
    /// </summary>
    [SerializeField] private TMP_Text tableText;
    /// <summary>
    /// Аниматор двери.
    /// </summary>
    [SerializeField] private Animator animator;
    /// <summary>
    /// Закрыта ли дверь.
    /// </summary>
    [SerializeField] private bool locked;
    /// <summary>
    /// Спрайт замка на двери.
    /// </summary>
    [SerializeField] private GameObject lockSprite;
    /// <summary>
    /// ID двери, должен быть уникальным.
    /// </summary>
    [SerializeField] public int doorID;
    /// <summary>
    /// Всплывающее уведомление.
    /// </summary>
    [SerializeField] private Popup popup;
    /// <summary>
    /// Обхект свойств игрока.
    /// </summary>
    private PlayerProperties playerProperties;

    /// <summary>
    /// Устанавливает название уровня и щакрывает дверь.
    /// </summary>
    private void Start()
    {
        tableText.SetText(tableString);
        lockSprite.SetActive(locked);
    }

    /// <summary>
    /// Активирует всплывающее уведомление и проигрывает анимацию открытия, при вхождении игрока в триггер.
    /// </summary>
    /// <param name="collision">Collider2D объекта, вошедшего в триггер.</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && !locked)
        {
            animator.SetBool("DoorOpen", true);
            popup.SetPopup(true);
        }
    }

    /// <summary>
    /// Деактивирует всплывающее уведомление и проигрывает анимацию закрытия, при выходе игрока из триггера.
    /// </summary>
    /// <param name="collision">Collider2D объекта, вышедшего из триггера.</param>
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && !locked)
        {
            animator.SetBool("DoorOpen", false);
            popup.SetPopup(false);
        }
    }

    /// <summary>
    /// Заменяет текущую сцену на заданную при нажатии игроком кнопки.
    /// </summary>
    /// <param name="collision">Collider2D объекта, вошедшего в триггер.</param>
    private void OnTriggerStay2D(Collider2D collision)
    {
        playerProperties = collision.GetComponent<PlayerProperties>();

        if (playerProperties.allowInput && !locked && (Input.GetButtonDown("Interact") || Input.GetButton("Interact")))
        {
            collision.GetComponent<PlayerNetworkTalker>().CmdSetLevelID(doorID);

            collision.GetComponent<PlayerNetworkTalker>().CmdChangeScene(sceneName);
        }
    }

    /// <summary>
    /// Открывает/закрывает дверь.
    /// </summary>
    /// <param name="state">Состояние двери.</param>
    public void SetLock(bool state)
    {
        locked = state;
        lockSprite.SetActive(locked);
    }
}
