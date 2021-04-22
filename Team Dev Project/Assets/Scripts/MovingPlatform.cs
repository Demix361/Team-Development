using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
/// <summary>
/// Класс двигающихся платформ.
/// </summary>
/// <remarks>
/// Назначения направления движения платформы и его скорости.
/// </remarks>
public class MovingPlatform : NetworkBehaviour
{
    /// <summary>
    /// Изменение положения нижней левой точки платформы
    /// </summary>
    [SerializeField] private Transform _leftBottomPoint;
    /// <summary>
    /// Изменения положения правой верхней точки платформы
    /// </summary>
    [SerializeField] private Transform _rightTopPoint;
    /// <summary>
    /// rigidBody платформы.
    /// </summary>
    [SerializeField] private Rigidbody2D _rigidBody;
    /// <summary>
    /// Состояние горизонтальное передвижение.
    /// </summary>
    [SerializeField] private bool _horizontalMovement;
    /// <summary>
    /// Состояние вертикальное передвижение.
    /// </summary>
    [SerializeField] private bool _verticalMovement;
    /// <summary>
    /// Значение скорости горизонтального передвижения.
    /// </summary>
    [SerializeField] private float _horizontalSpeed = 0;
    /// <summary>
    /// Значение скорости вертикального передвижения.
    /// </summary>
    [SerializeField] private float _verticalSpeed = 0;
    /// <summary>
    /// Transform левый нижней точки
    /// </summary>
    [SerializeField] private Transform _leftBottomSpritePoint;
    /// <summary>
    /// Transform правой верхней точки.
    /// </summary>
    [SerializeField] private Transform _rightTopSpritePoint;
    /// <summary>
    /// Сглаживание движения платформы.
    /// </summary>
    [SerializeField] private float m_MovementSmoothing = .05f;
    /// <summary>
    /// Вектор скорости.
    /// </summary>
    private Vector3 m_Velocity = Vector3.zero;
    /// <summary>
    /// Состояние направление движений.
    /// </summary>
    private bool _positiveMovement = true;
    /// <summary>
    /// Состояние движения.
    /// </summary>
    private bool _startMoving = false;
    /// <summary>
    /// Таймер движения.
    /// </summary>
    private float _timeOffset = 3;
    /// <summary>
    /// Счетчик платформы.
    /// </summary>
    private float _count = 0;

    /// <summary>
    /// Определение состояния направления движения платформы.
    /// </summary>
    private void Start()
    {
        if (_horizontalMovement)
            _verticalMovement = false;
        if (_verticalMovement)
            _horizontalMovement = false;
    }
    /// <summary>
    /// Обновление расположения платформы.
    /// </summary>
    private void Update()
    {
        if (isServer && !_startMoving)
        {
            _count += Time.deltaTime;

            if (_count > _timeOffset)
            {
                CmdStartMoving();
                _count = 0;
            }
        }
        else if (isServer && _startMoving)
        {
            _count += Time.deltaTime;

            if (_count > _timeOffset)
            {
                CmdCorrectPosition(transform.position);
                _count = 0;
            }
        }
    }
    /// <summary>
    /// Фиксирование изменение расположения платформы.
    /// </summary>
    private void FixedUpdate()
    {
        if (_startMoving)
        {
            if (_horizontalMovement)
            {
                if (_positiveMovement)
                {
                    if (_rightTopSpritePoint.position.x > _rightTopPoint.position.x)
                        _positiveMovement = false;
                    else
                    {
                        Vector3 targetVelocity = new Vector2(_horizontalSpeed * Time.fixedDeltaTime, 0);
                        _rigidBody.velocity = Vector3.SmoothDamp(_rigidBody.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
                    }
                }
                else
                {
                    if (_leftBottomSpritePoint.position.x < _leftBottomPoint.position.x)
                        _positiveMovement = true;
                    else
                    {
                        Vector3 targetVelocity = new Vector2(-_horizontalSpeed * Time.fixedDeltaTime, 0);
                        _rigidBody.velocity = Vector3.SmoothDamp(_rigidBody.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
                    }
                }
            }

            if (_verticalMovement)
            {
                if (_positiveMovement)
                {
                    if (_rightTopSpritePoint.position.y > _rightTopPoint.position.y)
                        _positiveMovement = false;
                    else
                    {
                        Vector3 targetVelocity = new Vector2(0, _verticalSpeed * Time.fixedDeltaTime);
                        _rigidBody.velocity = Vector3.SmoothDamp(_rigidBody.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
                    }
                }
                else
                {
                    if (_leftBottomSpritePoint.position.y < _leftBottomPoint.position.y)
                        _positiveMovement = true;
                    else
                    {
                        Vector3 targetVelocity = new Vector2(0, -_verticalSpeed * Time.fixedDeltaTime);
                        _rigidBody.velocity = Vector3.SmoothDamp(_rigidBody.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
                    }
                }
            }
        }
    }
    /// <summary>
    /// Расположение платформы
    /// </summary>
    /// <param name="position">позиция платформы</param>
    [Command(requiresAuthority = false)]
    private void CmdCorrectPosition(Vector3 position)
    {
        RpcCorrectPosition(position);
    }
    /// <summary>
    /// Корректировка позиции платформы
    /// </summary>
    /// <param name="position">Command. Не требует прав на объект. Вызывается с клиента - работает на сервере.Позиция платформы.</param>
    [ClientRpc]
    private void RpcCorrectPosition(Vector3 position)
    {
        if (!isServer)
        {
            transform.position = position;
        }
    }
    /// <summary>
    /// Начало движения платформы
    /// </summary>
    /// <remarks>
    /// Command. Не требует прав на объект. Вызывается с клиента - работает на сервере.
    /// </remarks>
    [Command(requiresAuthority = false)]
    private void CmdStartMoving()
    {
        RpcStartMoving();
    }
    /// <summary>
    /// Изменения статуса движения платформы
    /// </summary>
    /// <remarks>
    /// RPC. Вызывается с сервера - работает на клиенте.
    /// </remarks>
    [ClientRpc]
    private void RpcStartMoving()
    {
        _startMoving = true;
    }
  
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        if (_horizontalMovement)
        {
            Vector3 pointLeft_1 = new Vector3(_leftBottomPoint.position.x, transform.position.y + 1, 0);
            Vector3 pointLeft_2 = new Vector3(_leftBottomPoint.position.x, transform.position.y - 1, 0);
            Vector3 pointRight_1 = new Vector3(_rightTopPoint.position.x, transform.position.y + 1, 0);
            Vector3 pointRight_2 = new Vector3(_rightTopPoint.position.x, transform.position.y - 1, 0);
            Gizmos.DrawLine(pointLeft_1, pointLeft_2);
            Gizmos.DrawLine(pointRight_1, pointRight_2);
        }
        else if (_verticalMovement)
        {
            Vector3 pointBottom_1 = new Vector3(transform.position.x + 1, _leftBottomPoint.position.y, 0);
            Vector3 pointBottom_2 = new Vector3(transform.position.x - 1, _leftBottomPoint.position.y, 0);
            Vector3 pointTop_1 = new Vector3(transform.position.x + 1, _rightTopPoint.position.y, 0);
            Vector3 pointTop_2 = new Vector3(transform.position.x - 1, _rightTopPoint.position.y, 0);
            Gizmos.DrawLine(pointBottom_1, pointBottom_2);
            Gizmos.DrawLine(pointTop_1, pointTop_2);
        }

    }

}
