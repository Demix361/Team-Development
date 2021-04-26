using UnityEngine;

/// <summary>
/// Класс пушечного ядра.
/// </summary>
/// <remarks>
/// Пушечное ядро, взрывающееся при столкновении с игроком или картой. Наносит урон и отталкивает игрока.
/// </remarks>
public class CannonBall : MonoBehaviour
{
    /// <summary>
    /// Аниматор пушечного ядра.
    /// </summary>
    [SerializeField] private Animator _animator;
    /// <summary>
    /// Сила, с которой ядро ооталкивает игрока.
    /// </summary>
    [SerializeField] private float _force;
    /// <summary>
    /// Урон, наносимый игроку.
    /// </summary>
    [SerializeField] private float _damage;

    /// <summary>
    /// Время, после которого ядро будет уничтожено.
    /// </summary>
    private int _lifeTime = 10;
    /// <summary>
    /// Таймер.
    /// </summary>
    private float _count = 0;
    /// <summary>
    /// Взорвалось ли ядро.
    /// </summary>
    private bool _exploded = false;

    /// <summary>
    /// Уничтожает ядро с задержкой при взрыве или моментально по истечении <see cref="_lifeTime"/>.
    /// </summary>
    void Update()
    {
        if (!_exploded)
        {
            _count += Time.deltaTime;
            if (_count > _lifeTime)
                Destroy(gameObject);
        }
        else
        {
            _count += Time.deltaTime;
            if (_count > 0.54)
                Destroy(gameObject);
        }
    }

    /// <summary>
    /// Взрывает ядро при столкновении с картой или игроком. При столкновении с игроком, наносит ему урон и отталкивает его.
    /// </summary>
    /// <param name="collision">Collider2D объекта, вошедшего в триггер.</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((collision.tag == "Player" || collision.tag == "Tilemap") && _exploded == false)
        {
            _exploded = true;
            _count = 0;
            _animator.SetBool("Exploded", true);
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            GetComponent<Rigidbody2D>().angularVelocity = 0;

            if (collision.tag == "Player")
            {
                collision.GetComponent<Health>().CmdDealDamage(_damage);

                var forceVector = new Vector2(collision.transform.position.x - transform.position.x,
                    collision.transform.position.y - transform.position.y);
                forceVector.Normalize();

                collision.GetComponent<Rigidbody2D>().AddForce(forceVector * _force);
            }
        }
    }
}
