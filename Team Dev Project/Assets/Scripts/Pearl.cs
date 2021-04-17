using UnityEngine;

public class Pearl : MonoBehaviour
{
    [SerializeField] private float _force;
    [SerializeField] private float _damage;
    [SerializeField] private Animator _animator;

    private int _lifeTime = 10;
    private float _count = 0;
    private bool _destroyed = false;

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
                collision.GetComponent<Health>().CmdDealDamage(0);

                var forceVector = new Vector2(collision.transform.position.x - transform.position.x,
                    collision.transform.position.y - transform.position.y);
                forceVector.Normalize();

                collision.GetComponent<Rigidbody2D>().AddForce(forceVector * _force);
            }

            //Destroy(gameObject);
        }
    }
}
