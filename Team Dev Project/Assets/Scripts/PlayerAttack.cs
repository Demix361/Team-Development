using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

/// <summary>
/// Класс атаки игрока.
/// </summary>
/// <remarks>
/// Игрок бьет с заданым уроном и интервалом.
/// </remarks>
public class PlayerAttack : NetworkBehaviour
{
    /// <summary>
    /// Свойства игрока
    /// </summary>
    [SerializeField] private PlayerProperties _playerProperties;
    /// <summary>
    /// Аниматор атаки.
    /// </summary>
    [SerializeField] private Animator _animator;
    /// <summary>
    /// Трансформ атаки.
    /// </summary>
    [SerializeField] private Transform _attackColliderPoint1;
    /// <summary>
    /// Трансфом атаки.
    /// </summary>
    [SerializeField] private Transform _attackColliderPoint2;
    /// <summary>
    /// Маска уровней.
    /// </summary>
    [SerializeField] private LayerMask _layerMask;
    /// <summary>
    ///  Урон атаки.
    /// </summary>
    [SerializeField] private float _damage;
    /// <summary>
    /// Интервал между атаками.
    /// </summary>
    [SerializeField] private float _interval;

    /// <summary>
    /// Счетчик.
    /// </summary>
    private float _count = 0;

    /// <summary>
    /// Нанесение урона и отталкивание врага при атаке и попадании
    /// </summary>
    private void Update()
    {
        if (!hasAuthority)
            return;

        _count += Time.deltaTime;

        if (_playerProperties.allowInput && Input.GetButtonDown("Attack") && _count > _interval)
        {
            _animator.SetTrigger("Attack");
            _count = 0;

            Collider2D[] colliders = Physics2D.OverlapAreaAll(_attackColliderPoint1.position, _attackColliderPoint2.position, _layerMask);
            foreach (var collider in colliders)
            {
                EnemyHealth enemyHealth = collider.GetComponent<EnemyHealth>();
                if (enemyHealth)
                {
                    enemyHealth.CmdDealDamage(_damage);
                }
            }
        }
    }

    /// <summary>
    /// Окрашивание в красный при попадании
    /// </summary>
    private void OnDrawGizmos()
    {
        DrawRect(_attackColliderPoint1, _attackColliderPoint2, Color.red);
    }

    /// <summary>
    /// Изменение цветов
    /// </summary>
    /// <param name="point1">Transform точки1</param>
    /// <param name="point2">Transform точки2</param>
    /// <param name="color">Цвет</param>
    private void DrawRect(Transform point1, Transform point2, Color color)
    {
        Gizmos.color = color;
        Gizmos.DrawLine(point1.position, new Vector2(point1.position.x, point2.position.y));
        Gizmos.DrawLine(point1.position, new Vector2(point2.position.x, point1.position.y));
        Gizmos.DrawLine(point2.position, new Vector2(point1.position.x, point2.position.y));
        Gizmos.DrawLine(point2.position, new Vector2(point2.position.x, point1.position.y));
    }
}
