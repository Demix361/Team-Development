using UnityEngine;


/// <summary>
/// Класс жемчужин.
/// </summary>
/// <remarks>
/// Жемчужины вылетают и при столкновении с игроком отталкивают на заданное расстояние и наносят заданный урон.
/// </remarks>
public class Pearl : MonoBehaviour
{
    /// <summary>
    /// Cила отталкивания.
    /// </summary>
    [SerializeField] private float _force;
    /// <summary>
    /// Урон при попадании.
    /// </summary>
    [SerializeField] private float _damage;

    /// <summary>
    /// Аниматор жемчужины. 
    /// </summary>
    [SerializeField] private Animator _animator;

    /// <summary>
    /// Время жизни. 
    /// </summary>
    private int _lifeTime = 10;
    /// <summary>
    /// Счетчик.
    /// </summary>
    private float _count = 0;
    /// <summary>
    /// Состояние разрушенности
    /// </summary>
    private bool _destroyed = false;

    /// <summary>
    /// Разрушает игровой объект по истечению времени
    /// </summary>
    private void Update()
    {
        if (!_destroyed)
        {
            _count += Time.deltaTime;
            if (_count > _lifeTime)
                Destroy(gameObject);
        }
        else
        {
            _count += Time.deltaTime;
            if (_count > 0.25)
                Destroy(gameObject);
        }
    }

    /// <summary>
    /// Определяет направления выталкивания
    /// </summary>
    /// <param name="collision">Collider2D объекта, входящего в триггере.</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((collision.tag == "Player" || collision.tag == "Tilemap") && _destroyed == false)
        {
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            GetComponent<Rigidbody2D>().angularVelocity = 0;
            _destroyed = true;
            _count = 0;
            _animator.SetBool("Destroyed", true);

            if (collision.tag == "Player")
            {
                collision.GetComponent<Health>().CmdDealDamage(_damage);

                var forceVector = new Vector2(collision.transform.position.x - transform.position.x,
                    collision.transform.position.y - transform.position.y);
                forceVector.Normalize();

                collision.GetComponent<Rigidbody2D>().AddForce(forceVector * _force);
            }

            //Destroy(gameObject);
        }
    }
}
