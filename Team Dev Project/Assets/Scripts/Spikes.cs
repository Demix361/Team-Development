using UnityEngine;
/// <summary>
/// Класс Шипы.
/// </summary>
/// <remarks>
/// Шипы наносят урон в случае пересечения игрока с ними.
/// </remarks>
public class Spikes : MonoBehaviour
{
    /// <summary>
    /// Значение урона.
    /// </summary>
    [SerializeField] private int _damage;
    /// <summary>
    /// Значения перезарядки атаки.
    /// </summary>
    [SerializeField] private float _attackCooldown;
    /// <summary>
    /// Значения отталкивания игрока.
    /// </summary>
    [SerializeField] private float _force;
    /// <summary>
    /// Таймер шипов.
    /// </summary>
    static private float _spikesHitTimer;
    /// <summary>
    /// Значения обновления таймера.
    /// </summary>
    private bool _thisObjectUpdatingTimer = false;
    /// <summary>
    /// Назначение начальное значения таймера.
    /// </summary>
    private void Start()
    {
        _spikesHitTimer = 0;
    }
    /// <summary>
    /// Нанесение урона в случае с пересечением игрока и шипов.
    /// </summary>
    /// <param name="collision">Collider2D объекта, находящегося в триггере.</param>
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (_spikesHitTimer <= 0)
            {
                collision.GetComponent<Health>().CmdDealDamage(_damage);
                collision.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, _force));

                _spikesHitTimer = _attackCooldown;
                _thisObjectUpdatingTimer = true;
            }
        }
    }
    /// <summary>
    /// Обнвляет значения таймера.
    /// </summary>
    private void Update()
    {
        if (_thisObjectUpdatingTimer)
        {
            if (_spikesHitTimer > 0)
            {
                _spikesHitTimer -= Time.deltaTime;
            }
            else
            {
                _thisObjectUpdatingTimer = false;
            }
        }
    }
}
