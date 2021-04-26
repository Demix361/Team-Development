using UnityEngine;
using Mirror;

/// <summary>
/// Класс пушки.
/// </summary>
/// <remarks>
/// Пушка выпускает ядра с заданной периодичностью в горизонтальном направлении.
/// </remarks>
public class Cannon : NetworkBehaviour
{
    /// <summary>
    /// Аниматор пушки.
    /// </summary>
    [SerializeField] private Animator _animator;
    /// <summary>
    /// Аниматор дыма.
    /// </summary>
    [SerializeField] private Animator _smokeAnimator;
    /// <summary>
    /// Prefab ядра.
    /// </summary>
    [SerializeField] private GameObject _ballPrefab;
    /// <summary>
    /// Интервал стрельбы из пушки.
    /// </summary>
    [SerializeField] private float _shootInterval;
    /// <summary>
    /// Сдвиг времени выстрела, относительно интервала выстрела.
    /// </summary>
    [SerializeField] private float _shootOffset;
    /// <summary>
    /// Начальная сила ядра.
    /// </summary>
    [SerializeField] private float _shootForce;
    /// <summary>
    /// Transform точки появления ядра.
    /// </summary>
    [SerializeField] private Transform _ballSpawnPoint;

    /// <summary>
    /// Таймер.
    /// </summary>
    private float count;

    /// <summary>
    /// Устанавливает начальное время таймера.
    /// </summary>
    private void Start()
    {
        count = _shootOffset;
    }

    /// <summary>
    /// Вызывает <see cref="RpcShoot"/>
    /// </summary>
    /// <remarks>
    /// Command. Не требует прав на объект. Вызывается с клиента - работает на сервере.
    /// </remarks>
    [Command(requiresAuthority = false)]
    private void CmdShoot()
    {
        RpcShoot();
    }

    /// <summary>
    /// Совершает выстрел ядром из пушки.
    /// </summary>
    /// <remarks>
    /// RPC. Вызывается с сервера - работает на клиенте.
    /// </remarks>
    [ClientRpc]
    private void RpcShoot()
    {
        GameObject ball = Instantiate(_ballPrefab, _ballSpawnPoint.position, Quaternion.identity);
        ball.GetComponent<Rigidbody2D>().AddForce(new Vector2(_shootForce * -transform.localScale.x, 0));
        _animator.SetTrigger("Fire");
        _smokeAnimator.SetTrigger("Fire");
    }
    
    /// <summary>
    /// Вызывает <see cref="CmdShoot"/> с заданными перилдичностью и сдвигом.
    /// </summary>
    private void Update()
    {
        if (isServer)
        {
            count += Time.deltaTime;
            if (count > _shootInterval)
            {
                count = 0;
                CmdShoot();
            }
        }
    }
}
