using UnityEngine;
using Mirror;

/// <summary>
/// Класс контрольной точки.
/// </summary>
/// <remarks>
/// После активации позволяет игрокам возрождаться на данном объекте.
/// </remarks>
public class CheckPoint : NetworkBehaviour
{
    /// <summary>
    /// Аниматор флага
    /// </summary>
    [SerializeField] private Animator _animator;
    /// <summary>
    /// ID контрольной точки.
    /// </summary>
    /// <remarks>
    /// Должен быть уникальным среди всех контрольных точек на уровне.
    /// </remarks>
    [SerializeField] public int _checkpointID;
    /// <summary>
    /// Transform точки возрождения игрока.
    /// </summary>
    [SerializeField] public Transform _spawnPoint;
    /// <summary>
    /// Всплывающее уведомление.
    /// </summary>
    [SerializeField] private Popup _popup;

    /// <summary>
    /// Активирована ли контрольная точка.
    /// </summary>
    /// <remarks>
    /// Переменная синхронизированна.
    /// </remarks>
    [SyncVar] public bool _unlocked = false;

    /// <summary>
    /// Вызывает <see cref="CmdUnlockCheckpoint"/>, <see cref="CmdAddAllHearts"/> и восстанавливает здоровье всем игрокам при нажатии кнопки взаимодействия.
    /// </summary>
    /// <param name="collision">Collider2D объекта, находящегося в триггере.</param>
    private void OnTriggerStay2D(Collider2D collision)
    {
        PlayerProperties playerProperties = collision.GetComponent<PlayerProperties>();

        if (!_unlocked && playerProperties.allowInput && (Input.GetButtonDown("Interact") || Input.GetButton("Interact")))
        {
            CmdUnlockCheckpoint();
            CmdAddAllHearts();
            collision.GetComponent<Health>().CmdHealAllMax();
        }
    }

    /// <summary>
    /// Активирует всплывающее уведомление, при вхождении игрока в триггер.
    /// </summary>
    /// <param name="collision">Collider2D объекта, вошедшего в триггер.</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && !_unlocked)
        {
            _popup.SetPopup(true);
        }
    }

    /// <summary>
    /// Деактивирует всплывающее уведомление, при выходе игрока из триггера.
    /// </summary>
    /// <param name="collision">Collider2D объекта, вошедшего в триггер.</param>
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            _popup.SetPopup(false);
        }
    }

    /// <summary>
    /// Вызывает <see cref="HeartPanel.AddAllHearts"/>
    /// </summary>
    /// <remarks>
    /// Command. Не требует прав на объект. Вызывается с клиента - работает на сервере.
    /// </remarks>
    [Command(requiresAuthority = false)]
    private void CmdAddAllHearts()
    {
        GameObject.Find("HeartPanel").GetComponent<HeartPanel>().AddAllHearts();
    }

    /// <summary>
    /// Активирует контрольную точку, вызывает <see cref="RpcUnlockCheckpoint"/>.
    /// </summary>
    /// <remarks>
    /// Command. Не требует прав на объект. Вызывается с клиента - работает на сервере.
    /// </remarks>
    [Command(requiresAuthority = false)]
    private void CmdUnlockCheckpoint()
    {
        _unlocked = true;
        RpcUnlockCheckpoint();
    }

    /// <summary>
    /// Начинает анимацию активации контрольной точки.
    /// </summary>
    /// <remarks>
    /// RPC. Вызывается с сервера - работает на клиенте.
    /// </remarks>
    [ClientRpc]
    private void RpcUnlockCheckpoint()
    {
        _animator.SetBool("Set", true);
    }
}
