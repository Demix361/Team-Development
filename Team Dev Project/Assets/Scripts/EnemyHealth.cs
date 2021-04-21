using UnityEngine;
using Mirror;

/// <summary>
/// Класс здоровья врага.
/// </summary>
/// <remarks>
/// Отвечает за работу со здоровьем игрока.
/// </remarks>
public class EnemyHealth : NetworkBehaviour
{
    /// <summary>
    /// Максимальное здоровье.
    /// </summary>
    [SerializeField] private float _maxHealth;
    /// <summary>
    /// Неуязвим ли враг.
    /// </summary>
    [SerializeField] private bool _invincible = false;
    /// <summary>
    /// Текущее здоровье врага.
    /// </summary>
    /// <remarks>
    /// Синхронизирована.
    /// </remarks>
    [SyncVar] private float _currentHealth;

    /// <summary>
    /// Устанавливает максимальное значение здоровья.
    /// </summary>
    public override void OnStartServer()
    {
        SetHealth(_maxHealth);
    }

    /// <summary>
    /// Нанести урон врагу.
    /// </summary>
    /// <remarks>
    /// Command. Не требует прав на объект. Вызывается с клиента - работает на сервере.
    /// </remarks>
    /// <param name="damage"></param>
    [Command(requiresAuthority = false)]
    public void CmdDealDamage(float damage)
    {
        if (_invincible)
            return;

        RpcHitAnimation();
        SetHealth(Mathf.Max(_currentHealth - damage, 0));
    }

    /// <summary>
    /// Проиграть анимацию получения урона.
    /// </summary>
    /// <remarks>
    /// RPC. Вызывается с сервера - работает на клиенте.
    /// </remarks>
    [ClientRpc]
    private void RpcHitAnimation()
    {
        GetComponent<Animator>().SetTrigger("Hit");
    }

    /// <summary>
    /// Устанавливает здоровье врага.
    /// </summary>
    /// <remarks>
    /// Работает на сервере.
    /// </remarks>
    /// <param name="value">Значение здоровья.</param>
    [Server]
    private void SetHealth(float value)
    {
        if (value == 0)
        {
            Destroy(gameObject);
        }
        _currentHealth = value;
    }
}
