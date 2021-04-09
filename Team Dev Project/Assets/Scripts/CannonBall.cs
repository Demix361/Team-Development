using UnityEngine;

public class CannonBall : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private float _force;
    [SerializeField] private float _damage;

    private int _lifeTime = 10;
    private float _count = 0;
    private bool _exploded = false;

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
