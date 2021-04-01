using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBall : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    private int _lifeTime = 10;
    private float _count = 0;
    private float _speed = 0;
    private bool _exploded = false;

    public void Move(float speed)
    {
        _speed = speed;
    }

    void Update()
    {
        if (!_exploded)
        {
            //transform.position = new Vector3(transform.position.x + _speed, transform.position.y, transform.position.z);

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
        if (collision.tag == "Player" || collision.tag == "Tilemap")
        {
            _speed = 0;
            _exploded = true;
            _count = 0;
            _animator.SetBool("Exploded", true);
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            GetComponent<Rigidbody2D>().angularVelocity = 0;

            if (collision.tag == "Player")
            {
                collision.GetComponent<Health>().CmdDealDamage(20);
            }
        }
    }
}
