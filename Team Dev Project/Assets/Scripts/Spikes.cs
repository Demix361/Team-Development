using UnityEngine;

public class Spikes : MonoBehaviour
{
    [SerializeField] private int _damage;
    [SerializeField] private float _attackCooldown;
    [SerializeField] private float _force;
    static private float _spikesHitTimer;
    private bool _thisObjectUpdatingTimer = false;

    private void Start()
    {
        _spikesHitTimer = 0;
    }

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
