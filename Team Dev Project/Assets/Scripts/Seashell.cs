using UnityEngine;
using Mirror;

/// <summary>
/// Класс ракушки.
/// </summary>
/// <remarks>
/// Враг ракушка, атакующий в дальнем бою при приближении игрока и ближнем бою при нахождении игрока в непосредственной близости.
/// </remarks>
public class Seashell : NetworkBehaviour
{
    /// <summary>
    /// Первая точка Collider дальнего боя.
    /// </summary>
    [SerializeField] private Transform _rangeColliderPoint1;
    /// <summary>
    /// Вторая точка Collider дальнего боя.
    /// </summary>
    [SerializeField] private Transform _rangeColliderPoint2;
    /// <summary>
    /// Первая точка Collider ближнего боя.
    /// </summary>
    [SerializeField] private Transform _closeColliderPoint1;
    /// <summary>
    /// Вторая точка Collider ближнего боя.
    /// </summary>
    [SerializeField] private Transform _closeColliderPoint2;
    /// <summary>
    /// Маска слоев.
    /// </summary>
    [SerializeField] private LayerMask _layerMask;
    /// <summary>
    /// Аниматор ракушки.
    /// </summary>
    [SerializeField] private Animator _animator;
    
    /// <summary>
    /// Интервал атаки ближнего боя.
    /// </summary>
    [Header("Bite")]
    [SerializeField] private float _biteInterval;
    /// <summary>
    /// Урон атаки ближнего боя.
    /// </summary>
    [SerializeField] private float _biteDamage;
    /// <summary>
    /// Длительность анимации атаки ближнего боя.
    /// </summary>
    [SerializeField] private float _biteAnimationDuration;

    /// <summary>
    /// Интервал атаки дальнего боя.
    /// </summary>
    [Header("Pearl")]
    [SerializeField] private float _shootInterval;
    /// <summary>
    /// Prefab снаряда атаки дальнего боя.
    /// </summary>
    [SerializeField] private GameObject _pearlPrefab;
    /// <summary>
    /// Transform точки появления снаряда.
    /// </summary>
    [SerializeField] private Transform _pearlSpawnPoint;
    /// <summary>
    /// Начальная сила снаряда.
    /// </summary>
    [SerializeField] private float _pearlShootForce;
    /// <summary>
    /// Смещение интервала атаки.
    /// </summary>
    [SerializeField] private float _shootOffset;

    private float _shootCounter;
    private float _biteCounter;
    private bool _haveBitten;
    private bool _animationStarted;
    private bool _delayedShot;

    private void Update()
    {
        if (isServer)
        {
            _biteCounter += Time.deltaTime;
            _shootCounter += Time.deltaTime;
            _haveBitten = false;

            Collider2D[] colliders = Physics2D.OverlapAreaAll(_closeColliderPoint1.position, _closeColliderPoint2.position, _layerMask);
            if (colliders.Length > 0)
            {
                Collider2D closestCollider = colliders[0];
                foreach (var c in colliders)
                {
                    if (Mathf.Abs(c.transform.position.x - transform.position.x) < Mathf.Abs(closestCollider.transform.position.x - transform.position.x))
                    {
                        closestCollider = c;
                    }
                }

                // Turning around if closest player is behind
                if (transform.localScale.x > 0 && closestCollider.transform.position.x > transform.position.x)
                    CmdFlip();
                else if (transform.localScale.x < 0 && closestCollider.transform.position.x < transform.position.x)
                    CmdFlip();

                // Starting bite animation
                if (_biteCounter > _biteInterval && !_animationStarted)
                {
                    _biteCounter = 0;
                    _animationStarted = true;

                    for (int i = 0; i < colliders.Length; i++)
                    {
                        if (transform.localScale.x > 0 && colliders[i].transform.position.x < transform.position.x ||
                            transform.localScale.x < 0 && colliders[i].transform.position.x > transform.position.x)
                        {
                            CmdBite();
                        }
                    }
                }

                // Biting player at the end of animation if he is still in hitbox
                if (_biteCounter > _biteAnimationDuration && _animationStarted)
                {
                    if (_biteCounter < _biteAnimationDuration + 0.1)
                    {
                        for (int i = 0; i < colliders.Length; i++)
                        {
                            if (transform.localScale.x > 0 && colliders[i].transform.position.x < transform.position.x ||
                                transform.localScale.x < 0 && colliders[i].transform.position.x > transform.position.x)
                            {
                                colliders[i].GetComponent<Health>().CmdDealDamage(_biteDamage);
                            }
                        }
                    }

                    _biteCounter = 0;
                    _animationStarted = false;
                }

                _haveBitten = true;
            }

            colliders = Physics2D.OverlapAreaAll(_rangeColliderPoint1.position, _rangeColliderPoint2.position, _layerMask);
            
            // Shooting
            if (colliders.Length > 0)
            {
                if (_shootCounter > _shootInterval && !_haveBitten)
                {
                    _shootCounter = 0;
                    //CmdShoot();
                    _delayedShot = true;
                }

                if (_shootCounter  > _shootOffset && !_haveBitten &&_delayedShot)
                {
                    if (_shootCounter < _shootOffset + 0.1)
                        CmdShoot();
                    _shootCounter = 0;
                    _delayedShot = false;
                }
            }
        }
    }

    /// <summary>
    /// Вызывает <see cref="RpcBite"/>.
    /// </summary>
    [Command(requiresAuthority = false)]
    private void CmdBite()
    {
        RpcBite();
    }

    /// <summary>
    /// Запускает анимацию атаки ближнего боя.
    /// </summary>
    [ClientRpc]
    private void RpcBite()
    {
        _animator.SetTrigger("Bite");
    }

    /// <summary>
    /// Вызывает <see cref="RpcFlip"/>
    /// </summary>
    [Command(requiresAuthority = false)]
    private void CmdFlip()
    {
        RpcFlip();
    }

    /// <summary>
    /// Поворачивает по горизонтали ракушку.
    /// </summary>
    [ClientRpc]
    private void RpcFlip()
    {
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, 1);
    }

    /// <summary>
    /// Вызывает <see cref="RpcShoot"/>
    /// </summary>
    [Command(requiresAuthority = false)]
    private void CmdShoot()
    {
        RpcShoot();
    }

    /// <summary>
    /// Стреляет жемчужиной.
    /// </summary>
    [ClientRpc]
    private void RpcShoot()
    {
        GameObject pearl = Instantiate(_pearlPrefab, _pearlSpawnPoint.position, Quaternion.identity);
        pearl.GetComponent<Rigidbody2D>().AddForce(new Vector2(_pearlShootForce * -transform.localScale.x, 0));
        _animator.SetTrigger("Fire");
    }

    private void OnDrawGizmos()
    {
        DrawRect(_rangeColliderPoint1, _rangeColliderPoint2, Color.red);
        DrawRect(_closeColliderPoint1, _closeColliderPoint2, Color.blue);
    }

    private void DrawRect(Transform point1, Transform point2, Color color)
    {
        Gizmos.color = color;
        Gizmos.DrawLine(point1.position, new Vector2(point1.position.x, point2.position.y));
        Gizmos.DrawLine(point1.position, new Vector2(point2.position.x, point1.position.y));
        Gizmos.DrawLine(point2.position, new Vector2(point1.position.x, point2.position.y));
        Gizmos.DrawLine(point2.position, new Vector2(point2.position.x, point1.position.y));
    }
}
